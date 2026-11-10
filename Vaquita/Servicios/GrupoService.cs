using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Security.Cryptography;
using Vaquita.DTOs;
using Vaquita.Entidades;
using Vaquita.Excepciones;
using Vaquita.Utilidades;

namespace Vaquita.Servicios
{
    public class GrupoService : IGrupoService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly ISaldoService saldoService;

        public GrupoService(ApplicationDbContext context, IMapper mapper,
            IConfiguration configuration, IEmailService emailService, ISaldoService saldoService)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.emailService = emailService;
            this.saldoService = saldoService;
        }

        public async Task<GrupoDTO> Crear(int usuarioId, GrupoCreacionDTO grupoCreacion)
        {
            var ahora = DateTime.UtcNow;

            var grupo = mapper.Map<Grupo>(grupoCreacion);
            grupo.AdministradorId = usuarioId;
            grupo.Activo = true;
            grupo.FechaCreacion = ahora;

            grupo.Integrantes.Add(new Integrante
            {
                UsuarioId = usuarioId,
                EsAdministrador = true,
                FechaIngreso = ahora,
                UltimoAcceso = ahora
            });

            context.Add(grupo);
            await context.SaveChangesAsync();

            return await ObtenerPorId(grupo.Id, usuarioId);
        }

        // Lista desplegable: solo grupos ACTIVOS del usuario
        public async Task<List<GrupoListadoDTO>> ObtenerActivos(int usuarioId) =>
            await context.Integrantes
                .Where(i => i.UsuarioId == usuarioId && i.Grupo.Activo)
                .OrderByDescending(i => i.UltimoAcceso)
                .Select(i => new GrupoListadoDTO
                {
                    Id = i.GrupoId,
                    Descripcion = i.Grupo.Descripcion,
                    CantidadIntegrantes = i.Grupo.Integrantes.Count()
                })
                .ToListAsync();

        // Vista inicial: el último grupo en el que se trabajó
        public async Task<GrupoDTO?> ObtenerUltimo(int usuarioId)
        {
            var grupoId = await context.Integrantes
                .Where(i => i.UsuarioId == usuarioId && i.Grupo.Activo)
                .OrderByDescending(i => i.UltimoAcceso)
                .Select(i => (int?)i.GrupoId)
                .FirstOrDefaultAsync();

            if (grupoId is null)
            {
                return null;
            }

            return await ObtenerPorId(grupoId.Value, usuarioId);
        }

        public async Task<GrupoDTO> ObtenerPorId(int grupoId, int usuarioId)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId);

            await context.Integrantes
                .Where(i => i.GrupoId == grupoId && i.UsuarioId == usuarioId)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.UltimoAcceso, DateTime.UtcNow));

            return await context.Grupos
                .Where(g => g.Id == grupoId)
                .Select(g => new GrupoDTO
                {
                    Id = g.Id,
                    Descripcion = g.Descripcion,
                    Activo = g.Activo,
                    FechaCreacion = g.FechaCreacion,
                    FechaCierre = g.FechaCierre,
                    AdministradorId = g.AdministradorId,
                    Integrantes = g.Integrantes
                        .OrderBy(i => i.FechaIngreso).ThenBy(i => i.UsuarioId)
                        .Select(i => new IntegranteDTO
                        {
                            UsuarioId = i.UsuarioId,
                            Apodo = i.Usuario.Apodo,
                            Nombre = i.Usuario.Nombre,
                            Apellido = i.Usuario.Apellido,
                            EsAdministrador = i.EsAdministrador
                        }).ToList()
                })
                .FirstAsync();
        }

        // Punto de entrada común del QR y del link del mail de invitación
        public async Task Unirse(int grupoId, int usuarioId, string? tokenInvitacion)
        {
            var grupo = await context.Grupos.FirstOrDefaultAsync(g => g.Id == grupoId);

            if (grupo is null)
            {
                throw new NoEncontradoException("El grupo no existe.");
            }

            if (!grupo.Activo)
            {
                throw new ReglaDeNegocioException("El grupo está cerrado.");
            }

            if (!string.IsNullOrWhiteSpace(tokenInvitacion))
            {
                var invitacion = await context.Invitaciones.FirstOrDefaultAsync(i =>
                    i.Token == tokenInvitacion && i.GrupoId == grupoId
                    && i.Estado == EstadoInvitacion.Pendiente
                    && i.FechaVencimiento > DateTime.UtcNow);

                if (invitacion is null)
                {
                    throw new ReglaDeNegocioException("La invitación no es válida o ya venció.");
                }

                var emailUsuario = await context.Usuarios
                    .Where(u => u.Id == usuarioId).Select(u => u.Email).FirstAsync();

                if (!string.Equals(invitacion.Email, emailUsuario, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ReglaDeNegocioException(
                        "La invitación fue enviada a otro email.");
                }

                invitacion.Estado = EstadoInvitacion.Aceptada;
            }

            var yaEsIntegrante = await context.Integrantes
                .AnyAsync(i => i.GrupoId == grupoId && i.UsuarioId == usuarioId);

            if (!yaEsIntegrante)
            {
                var ahora = DateTime.UtcNow;

                context.Add(new Integrante
                {
                    GrupoId = grupoId,
                    UsuarioId = usuarioId,
                    EsAdministrador = false,
                    FechaIngreso = ahora,
                    UltimoAcceso = ahora
                });
            }

            await context.SaveChangesAsync();
        }

        // Invitación por mail (solo el administrador del grupo)
        public async Task Invitar(int grupoId, int usuarioId, InvitacionCreacionDTO invitacionCreacion)
        {
            var grupo = await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);

            if (grupo.AdministradorId != usuarioId)
            {
                throw new AccesoDenegadoException("Solo el administrador puede invitar integrantes.");
            }

            var email = invitacionCreacion.Email.Trim().ToLowerInvariant();

            var yaEsIntegrante = await context.Integrantes
                .AnyAsync(i => i.GrupoId == grupoId && i.Usuario.Email == email);

            if (yaEsIntegrante)
            {
                throw new ReglaDeNegocioException("Esa persona ya es integrante del grupo.");
            }

            var invitacion = new Invitacion
            {
                GrupoId = grupoId,
                InvitadoPorId = usuarioId,
                Email = email,
                Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
                Estado = EstadoInvitacion.Pendiente,
                FechaEnvio = DateTime.UtcNow,
                FechaVencimiento = DateTime.UtcNow.AddDays(7)
            };

            context.Add(invitacion);
            await context.SaveChangesAsync();

            // Mismo destino que el QR: /unirse/{grupoId} (con el token de la invitación)
            var link = $"{configuration["FrontendUrl"]}/unirse/{grupoId}?token={invitacion.Token}";

            var cuerpo = $"<p>Te invitaron a unirte al grupo <b>{grupo.Descripcion}</b> en Vaquita.</p>" +
                         $"<p>Aceptá la invitación acá: <a href=\"{link}\">{link}</a></p>" +
                         "<p>Tenés que estar registrado en la app para unirte.</p>";

            await emailService.Enviar(email, $"Invitación al grupo {grupo.Descripcion}", cuerpo);
        }

        // QR con la URL que redirige al "punto de unión" de la app
        private byte[] GenerarQr(int grupoId)
        {
            var url = $"{configuration["FrontendUrl"]}/unirse/{grupoId}";

            using var generador = new QRCodeGenerator();
            using var datos = generador.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qr = new PngByteQRCode(datos);
            return qr.GetGraphic(20);
        }

        public async Task<byte[]> ObtenerQr(int grupoId, int usuarioId)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);
            return GenerarQr(grupoId);
        }

        // Cerrar grupo: todos los saldos deben estar en cero
        public async Task Cerrar(int grupoId, int usuarioId)
        {
            var grupo = await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);

            if (grupo.AdministradorId != usuarioId)
            {
                throw new AccesoDenegadoException("Solo el administrador puede cerrar el grupo.");
            }

            var resumen = await saldoService.Calcular(grupoId);

            if (!resumen.Saldado)
            {
                throw new ReglaDeNegocioException(
                    "Para cerrar el grupo todos los saldos deben estar en cero.");
            }

            grupo.Activo = false;
            grupo.FechaCierre = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
}
