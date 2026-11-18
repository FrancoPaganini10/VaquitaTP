using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vaquita.DTOs;
using Vaquita.Entidades;
using Vaquita.Excepciones;
using Vaquita.Utilidades;

namespace Vaquita.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogger<UsuarioService> logger;

        public UsuarioService(ApplicationDbContext context, IConfiguration configuration,
            IMapper mapper, ILogger<UsuarioService> logger)
        {
            this.context = context;
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;
        }

        // Alta: guarda el usuario y envía un mail con el link de verificación
        public async Task Registrar(UsuarioCreacionDTO usuarioCreacion)
        {
            var email = usuarioCreacion.Email.Trim().ToLowerInvariant();

          
var existente = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

if (existente is not null)
{
    // No revelamos que el mail ya está registrado: se manda un link de acceso
    await EnviarLink(existente, TipoToken.Login);
    return;
}

            var usuario = mapper.Map<Usuario>(usuarioCreacion);
            usuario.Email = email;
            usuario.FechaAlta = DateTime.UtcNow;

            context.Add(usuario);
            await context.SaveChangesAsync();

            await EnviarLink(usuario, TipoToken.Alta);
        }

        // Login: envía un link de acceso al mail (si el usuario existe)
        public async Task SolicitarAcceso(string email)
        {
            email = email.Trim().ToLowerInvariant();

            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            // No se informa si el mail existe o no
            if (usuario is null)
            {
                return;
            }

            await EnviarLink(usuario, TipoToken.Login);
        }

        // Al presionar el link del mail: valida el token y devuelve el JWT
        public async Task<RespuestaAutenticacionDTO> Verificar(string token)
        {
            var hash = TokenHash.Hashear(token);

            var tokenVerificacion = await context.TokensVerificacion
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.TokenHash == hash && !t.Usado
                    && t.FechaVencimiento > DateTime.UtcNow);

            if (tokenVerificacion is null)
            {
                throw new ReglaDeNegocioException("El enlace no es válido o ya venció.");
            }

            tokenVerificacion.Usado = true;
            tokenVerificacion.Usuario.EmailVerificado = true;
            await context.SaveChangesAsync();

            return ConstruirToken(tokenVerificacion.Usuario);
        }

        public async Task<UsuarioDTO> ObtenerActual(int usuarioId) =>
            await context.Usuarios
                .Where(u => u.Id == usuarioId)
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Apodo = u.Apodo,
                    Email = u.Email
                })
                .FirstOrDefaultAsync()
            ?? throw new NoEncontradoException("El usuario no existe.");

        public RespuestaAutenticacionDTO ConstruirToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            };

            var llave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtKey"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            // El usuario "permanece logueado" en el dispositivo: token de larga duración
            var dias = configuration.GetValue("JwtDias", 30);
            var expiracion = DateTime.UtcNow.AddDays(dias);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: expiracion,
                signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new RespuestaAutenticacionDTO { Token = token, Expiracion = expiracion };
        }

        public async Task EnviarCorreo(string destino, string asunto, string cuerpoHtml)
        {
            var host = configuration["Smtp:Host"];

            if (string.IsNullOrWhiteSpace(host))
            {
                logger.LogInformation("[EMAIL SIMULADO] Para: {Destino} | Asunto: {Asunto} | Cuerpo: {Cuerpo}",
                    destino, asunto, cuerpoHtml);
                return;
            }

            var puerto = configuration.GetValue("Smtp:Port", 587);
            var desde = configuration["Smtp:Desde"]!;

            using var cliente = new SmtpClient(host, puerto)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    configuration["Smtp:Usuario"], configuration["Smtp:Password"])
            };

            using var mensaje = new MailMessage(desde, destino, asunto, cuerpoHtml)
            {
                IsBodyHtml = true
            };

            await cliente.SendMailAsync(mensaje);
        }

        private async Task EnviarLink(Usuario usuario, TipoToken tipo)
        {
            // Cooldown por destinatario: si ya se le mandó un link hace menos de 1 minuto no se
            // manda otro (evita usar la app para llenarle la casilla a alguien desde varias IP).
            // El token se creó 24 h antes de su vencimiento.
            var limite = DateTime.UtcNow.AddHours(24).AddMinutes(-1);
            var envioReciente = await context.TokensVerificacion
                .AnyAsync(t => t.UsuarioId == usuario.Id && t.FechaVencimiento > limite);

            if (envioReciente)
            {
                return;
            }

            // En la base se guarda solo el hash; el token real viaja únicamente en el mail
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            var tokenVerificacion = new TokenVerificacion
            {
                UsuarioId = usuario.Id,
                Tipo = tipo,
                TokenHash = TokenHash.Hashear(token),
                FechaVencimiento = DateTime.UtcNow.AddHours(24)
            };

            context.Add(tokenVerificacion);
            await context.SaveChangesAsync();

            var link = $"{configuration["FrontendUrl"]}/verificar?token={token}";

            var asunto = tipo == TipoToken.Alta
                ? "Confirmá tu cuenta en Vaquita"
                : "Tu enlace de acceso a Vaquita";

var cuerpo = $"<p>Hola {WebUtility.HtmlEncode(usuario.Apodo)},</p>" +                         $"<p>Ingresá a Vaquita con este enlace: <a href=\"{link}\">{link}</a></p>" +
                         "<p>El enlace vence en 24 horas.</p>";

            await EnviarCorreo(usuario.Email, asunto, cuerpo);
        }
    }
}
