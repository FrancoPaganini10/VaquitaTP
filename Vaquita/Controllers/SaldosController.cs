using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaquita.DTOs;
using Vaquita.Servicios;
using Vaquita.Utilidades;

namespace Vaquita.Controllers
{
    [ApiController]
    [Route("api/grupos/{grupoId:int}/saldos")]
    [Authorize]
    public class SaldosController : ControllerBase
    {
        private readonly ISaldoService servicio;

        public SaldosController(ISaldoService servicio)
        {
            this.servicio = servicio;
        }

        /// <summary>
        /// Resumen del grupo: Cuadro 1 (total de gastos y saldo por persona) y Cuadro 2 (quién le debe a quién).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResumenSaldosDTO>> Get(int grupoId) =>
            await servicio.ObtenerResumen(grupoId, User.ObtenerUsuarioId());
    }
}
