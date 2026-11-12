import { Link, Navigate } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import { useFetch } from "../hooks/useFetch.js";

// La app arranca en el último grupo en el que se trabajó
export default function Inicio() {
  const { data: ultimo, error, loading } = useFetch(() => gruposApi.ultimo(), []);

  if (loading) return <Cargando />;
  if (error) return <ErrorBanner error={error} />;
  if (ultimo) return <Navigate to={`/grupos/${ultimo.id}/gastos`} replace />;

  return (
    <section className="glass panel vacio">
      <h1>Todavía no tenés grupos</h1>
      <p>
        Creá uno para empezar a anotar gastos, o pedile a quien lo administra que te pase el QR o te invite por mail.
      </p>
      <Link className="btn btn-green" to="/grupos/nuevo">
        Crear mi primer grupo
      </Link>
    </section>
  );
}
