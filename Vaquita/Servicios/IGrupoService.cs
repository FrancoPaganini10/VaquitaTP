using Vaquita.DTOs;

namespace Vaquita.Servicios
{
    public interface IGrupoService
    {
        Task<GrupoDTO> Crear(int usuarioId, GrupoCreacionDTO grupoCreacion);
        Task<List<GrupoListadoDTO>> ObtenerActivos(int usuarioId);
        Task<GrupoDTO?> ObtenerUltimo(int usuarioId);
        Task<GrupoDTO> ObtenerPorId(int grupoId, int usuarioId);
        Task Unirse(int usuarioId, string? codigo, string? tokenInvitacion);
        Task Invitar(int grupoId, int usuarioId, InvitacionCreacionDTO invitacionCreacion);
        Task<byte[]> ObtenerQr(int grupoId, int usuarioId);
        Task Cerrar(int grupoId, int usuarioId);
    }
}
