import { moneda } from "../../lib/format.js";
import Panel from "../Panel.jsx";

// Cuadro 2: quién le debe a quién. Solo los involucrados pueden registrar el pago.
export default function ListaDeudas({ resumen, grupo, miId, onPagar }) {
  const { deudas, totalGrupo } = resumen;

  return (
    <Panel titulo="Quién le debe a quién">
      {deudas.length === 0 ? (
        <p className="ok-texto">
          {totalGrupo === 0 ? "Todavía no hay gastos cargados." : "Nadie debe nada: está todo saldado."}
        </p>
      ) : (
        <ul className="deudas">
          {deudas.map((d) => {
            const puedoRegistrar = grupo.activo && (miId === d.deudorId || miId === d.acreedorId);
            return (
              <li key={`${d.deudorId}-${d.acreedorId}`}>
                <span className="deuda-texto">
                  <strong>{d.deudor}</strong>
                  <span className="sr-only"> le debe a </span>
                  <span aria-hidden="true" className="flecha">{" > "}</span>
                  <strong>{d.acreedor}</strong>
                </span>
                <span className="deuda-importe">{moneda(d.importe)}</span>
                {puedoRegistrar && (
                  <button type="button" className="btn btn-small" onClick={() => onPagar(d)}>
                    Registrar pago
                  </button>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </Panel>
  );
}
