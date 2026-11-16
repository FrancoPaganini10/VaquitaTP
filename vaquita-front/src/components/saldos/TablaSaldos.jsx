import { moneda, saldoConSigno } from "../../lib/format.js";
import Panel from "../Panel.jsx";

function claseSaldo(saldo) {
  if (saldo > 0) return "pos";
  if (saldo < 0) return "neg";
  return "";
}

// Cuadro 1: cuánto gastó cada persona y su saldo contra la cuota del grupo.
export default function TablaSaldos({ resumen, miId }) {
  return (
    <Panel titulo="Saldos">
      <div className="tabla-wrap">
        <table className="tabla">
          <thead>
            <tr>
              <th scope="col">Persona</th>
              <th scope="col" className="num">Total gastos</th>
              <th scope="col" className="num">Saldo</th>
            </tr>
          </thead>
          <tbody>
            {resumen.saldos.map((s) => (
              <tr key={s.usuarioId}>
                <td>
                  {s.apodo}
                  {s.usuarioId === miId && <small className="vos"> (vos)</small>}
                </td>
                <td className="num">{moneda(s.totalGastos)}</td>
                <td className={`num ${claseSaldo(s.saldo)}`}>{saldoConSigno(s.saldo)}</td>
              </tr>
            ))}
          </tbody>
          <tfoot>
            <tr>
              <td>Total</td>
              <td className="num" colSpan={2}>
                {moneda(resumen.totalGrupo)} ({moneda(resumen.cuotaPorIntegrante)} c/u)
              </td>
            </tr>
          </tfoot>
        </table>
      </div>
      <p className="hint">Un saldo positivo es plata que le deben; uno negativo, plata que debe.</p>
    </Panel>
  );
}
