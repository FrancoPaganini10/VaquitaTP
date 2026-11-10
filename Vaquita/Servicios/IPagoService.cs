using Vaquita.DTOs;

namespace Vaquita.Servicios
{
    public interface IPagoService
    {
        Task<List<PagoDTO>> ObtenerPorGrupo(int grupoId, int usuarioId);
        Task<int> Crear(int grupoId, int usuarioId, PagoCreacionDTO pagoCreacion);
    }
}
