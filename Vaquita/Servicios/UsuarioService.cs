using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
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
        private readonly ILogger<UsuarioService> logger;

        public UsuarioService(ApplicationDbContext context, IConfiguration configuration,
            ILogger<UsuarioService> logger)
        {
            this.context = context;
            this.configuration = configuration;
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

            var usuario = new Usuario
            {
                Apellido = usuarioCreacion.Apellido,
                Nombre = usuarioCreacion.Nombre,
                Apodo = usuarioCreacion.Apodo,
                Email = email
            };
            usuario.FechaAlta = DateTime.UtcNow;

            context.Add(usuario);
            await context.SaveChangesAsync();

            await EnviarLink(usuario, TipoToken.Alta);
        }

        // Datos del usuario logueado (el JWT trae solo id y email)
        public async Task<UsuarioDTO> ObtenerYo(int usuarioId) =>
            await context.Usuarios
                .Where(u => u.Id == usuarioId)
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Apodo = u.Apodo,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email
                })
                .FirstOrDefaultAsync()
            ?? throw new NoEncontradoException("El usuario no existe.");

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
            var usuarioSmtp = configuration["Smtp:Usuario"];
            var passwordSmtp = configuration["Smtp:Password"];

            if (string.IsNullOrWhiteSpace(usuarioSmtp) || string.IsNullOrWhiteSpace(passwordSmtp))
            {
                throw new InvalidOperationException(
                    "La configuración SMTP requiere Smtp:Usuario y Smtp:Password.");
            }

            var mensaje = new MimeMessage();
            mensaje.From.Add(MailboxAddress.Parse(configuration["Smtp:Desde"]!));
            mensaje.To.Add(MailboxAddress.Parse(destino));
            mensaje.Subject = asunto;
            mensaje.Body = new BodyBuilder { HtmlBody = cuerpoHtml }.ToMessageBody();

            using var cliente = new MailKit.Net.Smtp.SmtpClient();
            var seguridad = puerto == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await cliente.ConnectAsync(host, puerto, seguridad);
            await cliente.AuthenticateAsync(usuarioSmtp, passwordSmtp);
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);
        }

        private async Task EnviarLink(Usuario usuario, TipoToken tipo)
        {    // Limpiar los tokens que ya vencieron
            await context.TokensVerificacion
                .Where(t => t.FechaVencimiento <= DateTime.UtcNow)
                .ExecuteDeleteAsync();
            // El token original se envía por correo; en la base se guarda únicamente su hash.
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

            var cuerpo = $"<p>Hola {WebUtility.HtmlEncode(usuario.Apodo)},</p>" +
                         $"<p>Ingresá a Vaquita con este enlace: <a href=\"{link}\">{link}</a></p>" +
                         "<p>El enlace vence en 24 horas.</p>";

            try
            {
                await EnviarCorreo(usuario.Email, asunto, cuerpo);
            }
            catch (Exception ex)
            {
                // El usuario y el token ya están guardados; puede pedir otro enlace con "acceso"
                logger.LogError(ex, "No se pudo enviar el mail a {Email}", usuario.Email);
            }
        }
    }
}