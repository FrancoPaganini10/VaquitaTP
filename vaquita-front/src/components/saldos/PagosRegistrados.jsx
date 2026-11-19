import { fecha, moneda } from "../../lib/format.js";
import Panel from "../Panel.jsx";

// Anula quien participó del pago (deudor o acreedor) o el administrador, y solo con el grupo abierto.
export default function PagosRegistrados({ pagos, grupo, miId, soyAdmin, onAnular }) {
  return (
    <Panel titulo="Pagos registrados">
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
                    {grupo.activo && (soyAdmin || miId === p.deudorId || miId === p.acreedorId) && (
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