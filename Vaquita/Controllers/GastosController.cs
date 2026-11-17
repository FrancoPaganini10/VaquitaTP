using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaquita.DTOs;
using Vaquita.Servicios;
using Vaquita.Utilidades;

namespace Vaquita.Controllers
{
    [ApiController]
    [Route("api/grupos/{grupoId:int}/gastos")]
    [Authorize]
    public class GastosController : ControllerBase
    {
        private readonly IGastoService servicio;

        public GastosController(IGastoService servicio)
        {
            this.servicio = servicio;
        }

        /// <summary>
        /// Todos los gastos del grupo (usuario, detalle e importe).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<GastoDTO>>> Get(int grupoId) =>
            await servicio.ObtenerPorGrupo(grupoId, User.ObtenerUsuarioId());

        /// <summary>
        /// Total gastado por rubro (los gastos sin rubro se agrupan en "Sin rubro").
        /// </summary>
        [HttpGet("por-rubro")]
        public async Task<ActionResult<List<TotalRubroDTO>>> GetPorRubro(int grupoId) =>
            await servicio.ObtenerTotalesPorRubro(grupoId, User.ObtenerUsuarioId());

        /// <summary>
        /// Registra un gasto propio o de otro integrante.
        /// </summary>
        /// <param name="grupoId">Id del grupo.</param>
        /// <param name="gastoCreacion">Importe positivo, descripción, rubro opcional y pagador opcional.</param>
        /// <response code="400">Importe inválido, grupo cerrado o pagador/rubro inexistente.</response>
        [HttpPost]
        public async Task<ActionResult> Post(int grupoId, GastoCreacionDTO gastoCreacion)
        {
            var id = await servicio.Crear(grupoId, User.ObtenerUsuarioId(), gastoCreacion);
            return Ok(new { id });
        }

        /// <summary>
        /// Anula un gasto registrado previamente. Solo puede anularlo el pagador o quien lo registró.
        /// </summary>
        /// <response code="400">El gasto ya está anulado o no tiene permisos para anularlo.</response>
        [HttpDelete("{grupoId:int}/gastos/{gastoId:int}")]
        public async Task<ActionResult> AnularGasto(int grupoId, int gastoId)
        {
            await servicio.Anular(grupoId, gastoId, User.ObtenerUsuarioId());
            return NoContent();
        }
    }
}
