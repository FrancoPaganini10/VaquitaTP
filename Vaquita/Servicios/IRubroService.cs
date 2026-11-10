using Vaquita.DTOs;

namespace Vaquita.Servicios
{
    public interface IRubroService
    {
        Task<List<RubroDTO>> ObtenerTodos();
    }
}
