import { useState } from "react";
import { useOutletContext } from "react-router-dom";
import { gastosApi, pagosApi, saldosApi } from "../api/endpoints.js";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import PagoModal from "../components/PagoModal.jsx";
import CerrarGrupo from "../components/saldos/CerrarGrupo.jsx";
import GastosPorRubro from "../components/saldos/GastosPorRubro.jsx";
import ListaDeudas from "../components/saldos/ListaDeudas.jsx";
import PagosRegistrados from "../components/saldos/PagosRegistrados.jsx";
import TablaSaldos from "../components/saldos/TablaSaldos.jsx";
import { useFetch } from "../hooks/useFetch.js";

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
        <TablaSaldos resumen={resumen} miId={miId} />
        <ListaDeudas resumen={resumen} grupo={grupo} miId={miId} onPagar={setDeudaAPagar} />
      </div>

      <div className="cuadros">
        <PagosRegistrados pagos={pagos} />
        <GastosPorRubro porRubro={porRubro} />
      </div>

      {grupo.activo && (
        <CerrarGrupo grupo={grupo} saldado={resumen.saldado} soyAdmin={soyAdmin} recargarGrupos={recargarGrupos} />
      )}

      <PagoModal deuda={deudaAPagar} onClose={() => setDeudaAPagar(null)} grupoId={grupo.id} onGuardado={alPagar} />
    </>
  );
}
