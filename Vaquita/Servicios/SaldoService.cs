using Microsoft.EntityFrameworkCore;
using Vaquita.DTOs;
using Vaquita.Utilidades;

namespace Vaquita.Servicios
{
    public class SaldoService : ISaldoService
    {
        private readonly ApplicationDbContext context;

        public SaldoService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ResumenSaldosDTO> ObtenerResumen(int grupoId, int usuarioId)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId);
            return await Calcular(grupoId);
        }

        public async Task<ResumenSaldosDTO> Calcular(int grupoId)
        {
            var integrantes = await context.Integrantes
                .Where(i => i.GrupoId == grupoId)
                .OrderBy(i => i.FechaIngreso).ThenBy(i => i.UsuarioId)
                .Select(i => new { i.UsuarioId, i.Usuario.Apodo })
                .ToListAsync();

            var gastosPorUsuario = await context.Gastos
                .Where(g => g.GrupoId == grupoId)
                .GroupBy(g => g.PagadorId)
                .Select(x => new { UsuarioId = x.Key, Total = x.Sum(g => g.Importe) })
                .ToDictionaryAsync(x => x.UsuarioId, x => x.Total);

            var pagosHechos = await context.Pagos
                .Where(p => p.GrupoId == grupoId)
                .GroupBy(p => p.DeudorId)
                .Select(x => new { UsuarioId = x.Key, Total = x.Sum(p => p.Importe) })
                .ToDictionaryAsync(x => x.UsuarioId, x => x.Total);

            var pagosRecibidos = await context.Pagos
                .Where(p => p.GrupoId == grupoId)
                .GroupBy(p => p.AcreedorId)
                .Select(x => new { UsuarioId = x.Key, Total = x.Sum(p => p.Importe) })
                .ToDictionaryAsync(x => x.UsuarioId, x => x.Total);

            var resumen = new ResumenSaldosDTO();

            if (integrantes.Count == 0)
            {
                return resumen;
            }

            var totalGrupo = gastosPorUsuario.Values.Sum();
            resumen.TotalGrupo = totalGrupo;
            resumen.CuotaPorIntegrante = Math.Round(totalGrupo / integrantes.Count, 2);

            // La cuota se reparte en centavos para que los saldos sumen exactamente cero
            // (los centavos sobrantes se asignan a los primeros integrantes).
            var totalCentavos = (long)Math.Round(totalGrupo * 100m);
            var cuotaBase = totalCentavos / integrantes.Count;
            var resto = totalCentavos % integrantes.Count;

            for (int i = 0; i < integrantes.Count; i++)
            {
                var integrante = integrantes[i];
                var cuota = (cuotaBase + (i < resto ? 1 : 0)) / 100m;

                gastosPorUsuario.TryGetValue(integrante.UsuarioId, out var pagado);
                pagosHechos.TryGetValue(integrante.UsuarioId, out var hechos);
                pagosRecibidos.TryGetValue(integrante.UsuarioId, out var recibidos);

                resumen.Saldos.Add(new SaldoIntegranteDTO
                {
                    UsuarioId = integrante.UsuarioId,
                    Apodo = integrante.Apodo,
                    TotalGastos = pagado,
                    Saldo = pagado - cuota + hechos - recibidos
                });
            }

            resumen.Saldado = resumen.Saldos.All(s => s.Saldo == 0);
            resumen.Deudas = CalcularDeudas(resumen.Saldos);

            return resumen;
        }

        // Cuadro 2: cada deudor (en orden) le paga al primer acreedor con saldo pendiente
        private static List<DeudaDTO> CalcularDeudas(List<SaldoIntegranteDTO> saldos)
        {
            var deudores = saldos.Where(s => s.Saldo < 0)
                .Select(s => new Pendiente { UsuarioId = s.UsuarioId, Apodo = s.Apodo, Monto = -s.Saldo })
                .ToList();

            var acreedores = saldos.Where(s => s.Saldo > 0)
                .Select(s => new Pendiente { UsuarioId = s.UsuarioId, Apodo = s.Apodo, Monto = s.Saldo })
                .ToList();

            var deudas = new List<DeudaDTO>();

            foreach (var deudor in deudores)
            {
                foreach (var acreedor in acreedores)
                {
                    if (deudor.Monto == 0)
                    {
                        break;
                    }

                    if (acreedor.Monto == 0)
                    {
                        continue;
                    }

                    var monto = Math.Min(deudor.Monto, acreedor.Monto);

                    deudas.Add(new DeudaDTO
                    {
                        DeudorId = deudor.UsuarioId,
                        Deudor = deudor.Apodo,
                        AcreedorId = acreedor.UsuarioId,
                        Acreedor = acreedor.Apodo,
                        Importe = monto
                    });

                    deudor.Monto -= monto;
                    acreedor.Monto -= monto;
                }
            }

            return deudas;
        }

        private class Pendiente
        {
            public int UsuarioId { get; set; }
            public string Apodo { get; set; } = null!;
            public decimal Monto { get; set; }
        }
    }
}
