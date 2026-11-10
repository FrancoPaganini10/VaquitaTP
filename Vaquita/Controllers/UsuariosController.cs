using Microsoft.AspNetCore.Mvc;
using Vaquita.DTOs;
using Vaquita.Servicios;

namespace Vaquita.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService servicio;

        public UsuariosController(IUsuarioService servicio)
        {
            this.servicio = servicio;
        }

        /// <summary>
        /// Alta de usuario. Envía un mail con un link para ingresar a la aplicación.
        /// </summary>
        /// <response code="200">Usuario creado, se envió el mail.</response>
        /// <response code="400">Ya existe un usuario con ese email.</response>
        [HttpPost("registrar")]
        public async Task<ActionResult> Registrar(UsuarioCreacionDTO usuarioCreacion)
        {
            await servicio.Registrar(usuarioCreacion);
            return Ok();
        }

        /// <summary>
        /// Login: envía al mail del usuario un link de acceso.
        /// </summary>
        [HttpPost("acceso")]
        public async Task<ActionResult> SolicitarAcceso(SolicitudAccesoDTO solicitud)
        {
            await servicio.SolicitarAcceso(solicitud.Email);
            return Ok();
        }

        /// <summary>
        /// Valida el token del link del mail (alta o login) y devuelve el JWT.
        /// </summary>
        /// <response code="200">Token JWT para usar en el header Authorization.</response>
        /// <response code="400">El enlace no es válido o ya venció.</response>
        [HttpPost("verificar")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Verificar(VerificarTokenDTO verificar) =>
            await servicio.Verificar(verificar.Token);
    }
}
