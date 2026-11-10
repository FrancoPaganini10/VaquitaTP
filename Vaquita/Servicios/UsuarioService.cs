using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vaquita.DTOs;
using Vaquita.Entidades;
using Vaquita.Excepciones;

namespace Vaquita.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;

        public UsuarioService(ApplicationDbContext context, IConfiguration configuration,
            IMapper mapper, IEmailService emailService)
        {
            this.context = context;
            this.configuration = configuration;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        // Alta: guarda el usuario y envía un mail con el link de verificación
        public async Task Registrar(UsuarioCreacionDTO usuarioCreacion)
        {
            var email = usuarioCreacion.Email.Trim().ToLowerInvariant();

            var yaExisteUsuarioConEsteEmail = await context.Usuarios.AnyAsync(u => u.Email == email);

            if (yaExisteUsuarioConEsteEmail)
            {
                throw new ReglaDeNegocioException(
                    "Ya existe un usuario con el email " + email + ". Pedí un enlace de acceso.");
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
            var tokenVerificacion = await context.TokensVerificacion
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Token == token && !t.Usado
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

        private async Task EnviarLink(Usuario usuario, TipoToken tipo)
        {
            var tokenVerificacion = new TokenVerificacion
            {
                UsuarioId = usuario.Id,
                Tipo = tipo,
                Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
                FechaVencimiento = DateTime.UtcNow.AddHours(24)
            };

            context.Add(tokenVerificacion);
            await context.SaveChangesAsync();

            var link = $"{configuration["FrontendUrl"]}/verificar?token={tokenVerificacion.Token}";

            var asunto = tipo == TipoToken.Alta
                ? "Confirmá tu cuenta en Vaquita"
                : "Tu enlace de acceso a Vaquita";

            var cuerpo = $"<p>Hola {usuario.Apodo},</p>" +
                         $"<p>Ingresá a Vaquita con este enlace: <a href=\"{link}\">{link}</a></p>" +
                         "<p>El enlace vence en 24 horas.</p>";

            await emailService.Enviar(usuario.Email, asunto, cuerpo);
        }
    }
}
