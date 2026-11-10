using Vaquita.DTOs;

namespace Vaquita.Servicios
{
    public interface IGastoService
    {
        Task<List<GastoDTO>> ObtenerPorGrupo(int grupoId, int usuarioId);
        Task<List<TotalRubroDTO>> ObtenerTotalesPorRubro(int grupoId, int usuarioId);
        Task<int> Crear(int grupoId, int usuarioId, GastoCreacionDTO gastoCreacion);
    }
}
