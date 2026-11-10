using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaquita.DTOs;
using Vaquita.Servicios;

namespace Vaquita.Controllers
{
    [ApiController]
    [Route("api/rubros")]
    [Authorize]
    public class RubrosController : ControllerBase
    {
        private readonly IRubroService servicio;

        public RubrosController(IRubroService servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<RubroDTO>>> Get() =>
            await servicio.ObtenerTodos();
    }
}
