import { useState } from "react";
import { useOutletContext } from "react-router-dom";
import { gastosApi } from "../api/endpoints.js";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import GastoModal from "../components/GastoModal.jsx";
import { useFetch } from "../hooks/useFetch.js";
import { fecha, moneda } from "../lib/format.js";

// Vista 1: todos los gastos del grupo (usuario, detalle e importe)
export default function GrupoGastos() {
  const { grupo, miId } = useOutletContext();
  const { data: gastos, error, loading, reload } = useFetch(() => gastosApi.listar(grupo.id), [grupo.id]);
  const [agregando, setAgregando] = useState(false);

  const total = gastos?.reduce((suma, g) => suma + g.importe, 0) ?? 0;

  return (
    <section className="glass panel">
      <div className="panel-head">
        <h2>Gastos del grupo</h2>
        {grupo.activo && (
          <button type="button" className="btn btn-green" onClick={() => setAgregando(true)}>
            Agregar gasto
          </button>
        )}
      </div>

      {loading && !gastos && <Cargando />}
      {error && <ErrorBanner error={error} />}

      {gastos && gastos.length === 0 && (
        <p className="vacio-texto">
          Todavía no hay gastos. {grupo.activo && "Anotá el primero con “Agregar gasto”."}
        </p>
      )}

      {gastos && gastos.length > 0 && (
        <div className="tabla-wrap">
          <table className="tabla">
            <thead>
              <tr>
                <th scope="col">Usuario</th>
                <th scope="col">Detalle</th>
                <th scope="col" className="num">
                  Importe
                </th>
              </tr>
            </thead>
            <tbody>
              {gastos.map((g) => (
                <tr key={g.id}>
                  <td>
                    {g.pagador}
                    {g.pagadorId === miId && <small className="vos"> (vos)</small>}
                  </td>
                  <td>
                    <span className="detalle">{g.descripcion}</span>
                    <span className="meta">
                      {fecha(g.fecha)}
                      {g.rubro && <span className="chip chip-rubro">{g.rubro}</span>}
                      {g.registradoPor !== g.pagador && <span> cargado por {g.registradoPor}</span>}
                    </span>
                  </td>
                  <td className="num">{moneda(g.importe)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr>
                <td colSpan={2}>Total</td>
                <td className="num">{moneda(total)}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      )}

      <GastoModal
        open={agregando}
        onClose={() => setAgregando(false)}
        grupo={grupo}
        miId={miId}
        onGuardado={reload}
      />
    </section>
  );
}
