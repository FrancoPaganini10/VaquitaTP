import { useState } from "react";
import { useOutletContext } from "react-router-dom";
import { gastosApi, pagosApi, saldosApi } from "../api/endpoints.js";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import AnularModal from "../components/AnularModal.jsx";
import PagoModal from "../components/PagoModal.jsx";
import CerrarGrupo from "../components/saldos/CerrarGrupo.jsx";
import GastosPorRubro from "../components/saldos/GastosPorRubro.jsx";
import ListaDeudas from "../components/saldos/ListaDeudas.jsx";
import PagosRegistrados from "../components/saldos/PagosRegistrados.jsx";
import TablaSaldos from "../components/saldos/TablaSaldos.jsx";
import { useFetch } from "../hooks/useFetch.js";
import { fecha, moneda } from "../lib/format.js";

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
  const [pagoAAnular, setPagoAAnular] = useState(null);

  if (loading && !resumen) return <Cargando />;
  if (error) return <ErrorBanner error={error} />;

  const alPagar = () => {
    recargarResumen();
    recargarPagos();
  };

  return (
    <>
      <div className="cuadros">
        <TablaSaldos resumen={resumen} miId={miId} />
        <ListaDeudas resumen={resumen} grupo={grupo} miId={miId} onPagar={setDeudaAPagar} />
      </div>

      <div className="cuadros">
        <PagosRegistrados
          pagos={pagos}
          grupo={grupo}
          miId={miId}
          soyAdmin={soyAdmin}
          onAnular={setPagoAAnular}
        />
        <GastosPorRubro porRubro={porRubro} />
      </div>

      {grupo.activo && (
        <CerrarGrupo grupo={grupo} saldado={resumen.saldado} soyAdmin={soyAdmin} recargarGrupos={recargarGrupos} />
      )}

      <PagoModal
        deuda={deudaAPagar}
        saldos={resumen.saldos}
        onClose={() => setDeudaAPagar(null)}
        grupoId={grupo.id}
        onGuardado={alPagar}
      />

      <AnularModal
        item={pagoAAnular}
        titulo="¿Anular el pago?"
        descripcion={(p) => (
          <>
            Se anula el pago de <strong>{moneda(p.importe)}</strong> de <strong>{p.deudor}</strong> a{" "}
            <strong>{p.acreedor}</strong> del {fecha(p.fecha)}. Los saldos se van a recalcular.
          </>
        )}
        onAnular={(p) => pagosApi.anular(grupo.id, p.id)}
        onClose={() => setPagoAAnular(null)}
        onAnulado={alPagar}
      />
    </>
  );
}