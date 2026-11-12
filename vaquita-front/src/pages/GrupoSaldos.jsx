import { useState } from "react";
import { useNavigate, useOutletContext } from "react-router-dom";
import { gastosApi, gruposApi, pagosApi, saldosApi } from "../api/endpoints.js";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import Modal from "../components/Modal.jsx";
import PagoModal from "../components/PagoModal.jsx";
import { useFetch } from "../hooks/useFetch.js";
import { fecha, moneda, saldoConSigno } from "../lib/format.js";

function CerrarGrupo({ grupo, saldado, soyAdmin, recargarGrupos }) {
  const navigate = useNavigate();
  const [confirmando, setConfirmando] = useState(false);
  const [error, setError] = useState("");
  const [cerrando, setCerrando] = useState(false);

  async function cerrar() {
    setCerrando(true);
    setError("");
    try {
      await gruposApi.cerrar(grupo.id);
      recargarGrupos();
      navigate("/");
    } catch (e) {
      setError(e.message);
      setCerrando(false);
    }
  }

  const puedeCerrar = soyAdmin && saldado;
  let motivo = "";
  if (!soyAdmin) motivo = "Solo el administrador puede cerrar el grupo.";
  else if (!saldado) motivo = "Para cerrar el grupo, todos los saldos tienen que estar en cero.";

  return (
    <section className="glass panel cerrar">
      <div>
        <h2>Cerrar grupo</h2>
        <p className={motivo ? "hint" : "ok-texto"}>
          {motivo || "Todos los saldos están en cero. Ya podés cerrar el grupo."}
        </p>
      </div>
      <button type="button" className="btn btn-ghost" disabled={!puedeCerrar} onClick={() => setConfirmando(true)}>
        Cerrar grupo
      </button>

      <Modal open={confirmando} onClose={() => setConfirmando(false)} titulo="¿Cerrar el grupo?">
        <p>
          Al cerrar <strong>{grupo.descripcion}</strong> ya no vas a poder cargar gastos ni pagos, y deja de aparecer
          en la lista de grupos activos.
        </p>
        {error && (
          <p className="banner banner-error" role="alert">
            {error}
          </p>
        )}
        <div className="acciones">
          <button type="button" className="btn btn-ghost" onClick={() => setConfirmando(false)}>
            Cancelar
          </button>
          <button type="button" className="btn" onClick={cerrar} disabled={cerrando}>
            {cerrando ? "Cerrando…" : "Cerrar grupo"}
          </button>
        </div>
      </Modal>
    </section>
  );
}

// Vista 2: Cuadro 1 (saldos) y Cuadro 2 (quién le debe a quién) en la misma página
export default function GrupoSaldos() {
  const { grupo, miId, soyAdmin, recargarGrupos } = useOutletContext();
  const { data: resumen, error, loading, reload: recargarResumen } = useFetch(
    () => saldosApi.obtener(grupo.id),
    [grupo.id]
  );
  const { data: pagos, reload: recargarPagos } = useFetch(() => pagosApi.listar(grupo.id), [grupo.id]);
  const { data: porRubro } = useFetch(() => gastosApi.porRubro(grupo.id), [grupo.id]);
  const [deudaAPagar, setDeudaAPagar] = useState(null);

  if (loading && !resumen) return <Cargando />;
  if (error) return <ErrorBanner error={error} />;

  const alPagar = () => {
    recargarResumen();
    recargarPagos();
  };

  return (
    <>
      <div className="cuadros">
        <section className="glass panel" aria-labelledby="t-saldos">
          <h2 id="t-saldos">Saldos</h2>
          <div className="tabla-wrap">
            <table className="tabla">
              <thead>
                <tr>
                  <th scope="col">Persona</th>
                  <th scope="col" className="num">
                    Total gastos
                  </th>
                  <th scope="col" className="num">
                    Saldo
                  </th>
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
                    <td className={`num ${s.saldo > 0 ? "pos" : s.saldo < 0 ? "neg" : ""}`}>
                      {saldoConSigno(s.saldo)}
                    </td>
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
        </section>

        <section className="glass panel" aria-labelledby="t-deudas">
          <h2 id="t-deudas">Quién le debe a quién</h2>
          {resumen.deudas.length === 0 ? (
            <p className="ok-texto">
              {resumen.totalGrupo === 0 ? "Todavía no hay gastos cargados." : "Nadie debe nada: está todo saldado."}
            </p>
          ) : (
            <ul className="deudas">
              {resumen.deudas.map((d) => {
                const puedoRegistrar = grupo.activo && (miId === d.deudorId || miId === d.acreedorId);
                return (
                  <li key={`${d.deudorId}-${d.acreedorId}`}>
                    <span className="deuda-texto">
                      <strong>{d.deudor}</strong>
                      <span className="sr-only"> le debe a </span>
                      <span aria-hidden="true" className="flecha">
                        {" > "}
                      </span>
                      <strong>{d.acreedor}</strong>
                    </span>
                    <span className="deuda-importe">{moneda(d.importe)}</span>
                    {puedoRegistrar && (
                      <button type="button" className="btn btn-small" onClick={() => setDeudaAPagar(d)}>
                        Registrar pago
                      </button>
                    )}
                  </li>
                );
              })}
            </ul>
          )}
        </section>
      </div>

      <div className="cuadros">
        <section className="glass panel" aria-labelledby="t-pagos">
          <h2 id="t-pagos">Pagos registrados</h2>
          {!pagos || pagos.length === 0 ? (
            <p className="vacio-texto">Todavía no se registraron pagos entre integrantes.</p>
          ) : (
            <div className="tabla-wrap">
              <table className="tabla">
                <thead>
                  <tr>
                    <th scope="col">Fecha</th>
                    <th scope="col">Pago</th>
                    <th scope="col" className="num">
                      Importe
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
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>

        <section className="glass panel" aria-labelledby="t-rubros">
          <h2 id="t-rubros">Gastos por rubro</h2>
          {!porRubro || porRubro.length === 0 ? (
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
        </section>
      </div>

      {grupo.activo && (
        <CerrarGrupo grupo={grupo} saldado={resumen.saldado} soyAdmin={soyAdmin} recargarGrupos={recargarGrupos} />
      )}

      <PagoModal
        deuda={deudaAPagar}
        onClose={() => setDeudaAPagar(null)}
        grupoId={grupo.id}
        onGuardado={alPagar}
      />
    </>
  );
}
