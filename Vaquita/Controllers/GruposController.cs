using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaquita.DTOs;
using Vaquita.Servicios;
using Vaquita.Utilidades;

namespace Vaquita.Controllers
{
    [ApiController]
    [Route("api/grupos")]
    [Authorize]
    public class GruposController : ControllerBase
    {
        private readonly IGrupoService servicio;

        public GruposController(IGrupoService servicio)
        {
            this.servicio = servicio;
        }

        /// <summary>
        /// Grupos ACTIVOS del usuario (lista desplegable).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<GrupoListadoDTO>>> Get() =>
            await servicio.ObtenerActivos(User.ObtenerUsuarioId());

        /// <summary>
        /// Último grupo en el que trabajó el usuario (vista inicial de la app).
        /// </summary>
        /// <response code="204">El usuario todavía no tiene grupos activos.</response>
        [HttpGet("ultimo")]
        public async Task<ActionResult<GrupoDTO>> GetUltimo()
        {
            var grupo = await servicio.ObtenerUltimo(User.ObtenerUsuarioId());

            if (grupo is null)
            {
                return NoContent();
            }

            return grupo;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GrupoDTO>> Get(int id) =>
            await servicio.ObtenerPorId(id, User.ObtenerUsuarioId());

        /// <summary>
        /// Crea un grupo. El usuario que lo crea queda como administrador.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GrupoDTO>> Post(GrupoCreacionDTO grupoCreacion)
        {
            var grupo = await servicio.Crear(User.ObtenerUsuarioId(), grupoCreacion);
            return CreatedAtAction(nameof(Get), new { id = grupo.Id }, grupo);
        }

        /// <summary>
        /// Incorpora al usuario logueado al grupo. Es el destino del QR y del link de la invitación por mail.
        /// </summary>
        /// <param name="id">Id del grupo (viene en el QR / link).</param>
        /// <param name="token">Token de la invitación (solo si viene del mail).</param>
        [HttpPost("{id:int}/unirse")]
        public async Task<ActionResult> Unirse(int id, [FromQuery] string? token)
        {
            await servicio.Unirse(id, User.ObtenerUsuarioId(), token);
            return Ok();
        }

        /// <summary>
        /// Invita a un nuevo integrante por mail (solo el administrador).
        /// </summary>
        [HttpPost("{id:int}/invitaciones")]
        public async Task<ActionResult> Invitar(int id, InvitacionCreacionDTO invitacionCreacion)
        {
            await servicio.Invitar(id, User.ObtenerUsuarioId(), invitacionCreacion);
            return Ok();
        }

        /// <summary>
        /// Genera el código QR (imagen PNG) para que otros usuarios se unan al grupo.
        /// </summary>
        [HttpGet("{id:int}/qr")]
        public async Task<IActionResult> GetQr(int id)
        {
            var png = await servicio.ObtenerQr(id, User.ObtenerUsuarioId());
            return File(png, "image/png");
        }

        /// <summary>
        /// Cierra el grupo. Todos los saldos deben estar en cero (solo el administrador).
        /// </summary>
        /// <response code="400">Hay saldos pendientes.</response>
        [HttpPost("{id:int}/cerrar")]
        public async Task<ActionResult> Cerrar(int id)
        {
            await servicio.Cerrar(id, User.ObtenerUsuarioId());
            return NoContent();
        }
    }
}
