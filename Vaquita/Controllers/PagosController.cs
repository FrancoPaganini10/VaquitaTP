using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaquita.DTOs;
using Vaquita.Servicios;
using Vaquita.Utilidades;

namespace Vaquita.Controllers
{
    [ApiController]
    [Route("api/grupos/{grupoId:int}/pagos")]
    [Authorize]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService servicio;

        public PagosController(IPagoService servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<PagoDTO>>> Get(int grupoId) =>
            await servicio.ObtenerPorGrupo(grupoId, User.ObtenerUsuarioId());

        /// <summary>
        /// Registra un pago (parcial o total) entre dos integrantes para saldar deudas.
        /// </summary>
        /// <response code="400">No hay deuda entre ellos o el pago supera lo adeudado.</response>
        [HttpPost]
        public async Task<ActionResult> Post(int grupoId, PagoCreacionDTO pagoCreacion)
        {
            var id = await servicio.Crear(grupoId, User.ObtenerUsuarioId(), pagoCreacion);
            return Ok(new { id });
        }
    }
}
