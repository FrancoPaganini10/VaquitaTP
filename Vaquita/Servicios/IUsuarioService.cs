using Vaquita.DTOs;
using Vaquita.Entidades;

namespace Vaquita.Servicios
{
    public interface IUsuarioService
    {
        Task Registrar(UsuarioCreacionDTO usuarioCreacion);
        Task SolicitarAcceso(string email);
        Task<RespuestaAutenticacionDTO> Verificar(string token);
        RespuestaAutenticacionDTO ConstruirToken(Usuario usuario);
        Task EnviarCorreo(string destino, string asunto, string cuerpoHtml);
    }
}
