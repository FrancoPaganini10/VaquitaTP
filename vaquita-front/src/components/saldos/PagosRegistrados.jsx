import { fecha, moneda } from "../../lib/format.js";
import Panel from "../Panel.jsx";

// `onNuevo` es null cuando el usuario no tiene ninguna deuda en la que participe.
// El back valida quién puede anular.
export default function PagosRegistrados({ pagos, grupo, onAnular, onNuevo }) {
  return (
    <Panel titulo="Pagos registrados">
      {grupo.activo && (
        <div className="acciones" style={{ marginTop: 0, marginBottom: "0.75rem" }}>
          <button
            type="button"
            className="btn btn-green btn-small"
            disabled={!onNuevo}
            title={onNuevo ? "" : "No hay deudas pendientes en las que participes"}
            onClick={onNuevo}
          >
            Registrar pago
          </button>
        </div>
      )}

      {!pagos?.length ? (
        <p className="vacio-texto">Todavía no se registraron pagos entre integrantes.</p>
      ) : (
        <div className="tabla-wrap">
          <table className="tabla">
            <thead>
              <tr>
                <th scope="col">Fecha</th>
                <th scope="col">Pago</th>
                <th scope="col" className="num">Importe</th>
                <th scope="col">
                  <span className="sr-only">Acciones</span>
                </th>
              </tr>
            </thead>
            <tbody>
              {pagos.map((p) => (
                <tr key={p.id}>
                  <td>{fecha(p.fecha)}</td>
                  <td>
                    {p.deudor} le pagó a {p.acreedor}
                  </td>
                  <td className="num">{moneda(p.importe)}</td>
                  <td>
                    {grupo.activo && (
                      <button type="button" className="btn btn-ghost btn-small" onClick={() => onAnular(p)}>
                        Anular
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </Panel>
  );
}