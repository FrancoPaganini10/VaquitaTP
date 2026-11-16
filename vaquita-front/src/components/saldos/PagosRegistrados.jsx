import { fecha, moneda } from "../../lib/format.js";
import Panel from "../Panel.jsx";

export default function PagosRegistrados({ pagos }) {
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
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </Panel>
  );
}
