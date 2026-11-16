import { moneda } from "../../lib/format.js";
import Panel from "../Panel.jsx";

export default function GastosPorRubro({ porRubro }) {
  return (
    <Panel titulo="Gastos por rubro">
      {!porRubro?.length ? (
        <p className="vacio-texto">Cuando haya gastos, acá vas a ver cuánto se gastó en cada rubro.</p>
      ) : (
        <ul className="rubros">
          {porRubro.map((r) => (
            <li key={r.rubro}>
              <span>{r.rubro}</span>
              <strong>{moneda(r.total)}</strong>
            </li>
          ))}
        </ul>
      )}
    </Panel>
  );
}
