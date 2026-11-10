using Vaquita.DTOs;

namespace Vaquita.Servicios
{
    public interface ISaldoService
    {
        // Valida que el usuario sea integrante y devuelve Cuadro 1 + Cuadro 2
        Task<ResumenSaldosDTO> ObtenerResumen(int grupoId, int usuarioId);

        // Cálculo puro, sin validar permisos (uso interno de otros servicios)
        Task<ResumenSaldosDTO> Calcular(int grupoId);
    }
}
