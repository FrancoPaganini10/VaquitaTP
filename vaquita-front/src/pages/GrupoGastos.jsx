import { useState } from "react";
import { useOutletContext } from "react-router-dom";
import { gastosApi } from "../api/endpoints.js";
import AnularModal from "../components/AnularModal.jsx";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import GastoModal from "../components/GastoModal.jsx";
import Panel from "../components/Panel.jsx";
import { useFetch } from "../hooks/useFetch.js";
import { fecha, moneda } from "../lib/format.js";

// Vista 1: lista de gastos del grupo + alta y anulación.
export default function GrupoGastos() {
  const { grupo, miId } = useOutletContext();
  const { data: gastos, error, loading, reload } = useFetch(() => gastosApi.listar(grupo.id), [grupo.id]);
  const [agregando, setAgregando] = useState(false);
  const [gastoAAnular, setGastoAAnular] = useState(null);

  if (loading && !gastos) return <Cargando />;
  if (error) return <ErrorBanner error={error} />;

  return (
    <>
      <Panel titulo="Gastos">
        {grupo.activo && (
          <div className="acciones" style={{ marginTop: 0, marginBottom: "0.75rem" }}>
            <button type="button" className="btn btn-green" onClick={() => setAgregando(true)}>
              Agregar gasto
            </button>
          </div>
        )}

        {!gastos?.length ? (
          <p className="vacio-texto">Todavía no hay gastos cargados.</p>
        ) : (
          <div className="tabla-wrap">
            <table className="tabla">
              <thead>
                <tr>
                  <th scope="col">Fecha</th>
                  <th scope="col">Descripción</th>
                  <th scope="col">Pagó</th>
                  <th scope="col">Rubro</th>
                  <th scope="col" className="num">Importe</th>
                  <th scope="col">
                    <span className="sr-only">Acciones</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {gastos.map((g) => (
                  <tr key={g.id}>
                    <td>{fecha(g.fecha)}</td>
                    <td>{g.descripcion}</td>
                    <td>{g.pagador}</td>
                    <td>{g.rubro ?? "—"}</td>
                    <td className="num">{moneda(g.importe)}</td>
                    <td>
                      {grupo.activo && (
                        <button type="button" className="btn btn-ghost btn-small" onClick={() => setGastoAAnular(g)}>
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

      <GastoModal open={agregando} onClose={() => setAgregando(false)} grupo={grupo} miId={miId} onGuardado={reload} />

      <AnularModal
        item={gastoAAnular}
        titulo="¿Anular el gasto?"
        descripcion={(g) => (
          <>
            Se anula el gasto <strong>{g.descripcion}</strong> de <strong>{moneda(g.importe)}</strong> pagado por{" "}
            <strong>{g.pagador}</strong>. Los saldos se van a recalcular.
          </>
        )}
        onAnular={(g) => gastosApi.anular(grupo.id, g.id)}
        onClose={() => setGastoAAnular(null)}
        onAnulado={reload}
      />
    </>
  );
}