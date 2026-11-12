import { Link, Outlet, useMatch, useNavigate } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import { useAuth } from "../auth/AuthContext.jsx";
import { useFetch } from "../hooks/useFetch.js";

// Barra superior: logo, lista desplegable de grupos ACTIVOS, nuevo grupo y salir.
export default function Layout() {
  const { sesion, cerrarSesion } = useAuth();
  const navigate = useNavigate();
  const match = useMatch("/grupos/:grupoId/*");
  const idEnUrl = match?.params.grupoId;
  const grupoActualId = /^\d+$/.test(idEnUrl ?? "") ? idEnUrl : "";

  const { data: grupos, reload: recargarGrupos } = useFetch(() => gruposApi.listar(), []);
  const estaEnLista = grupos?.some((g) => String(g.id) === grupoActualId);

  return (
    <>
      <header className="topbar glass">
        <Link to="/" className="logo">
          <span aria-hidden="true">🐮</span> Vaquita
        </Link>

        <label className="switcher">
          <span className="sr-only">Grupo actual</span>
          <select
            value={grupoActualId}
            onChange={(e) => e.target.value && navigate(`/grupos/${e.target.value}/gastos`)}
          >
            {!grupoActualId && (
              <option value="" disabled>
                {grupos?.length ? "Elegí un grupo" : "Sin grupos activos"}
              </option>
            )}
            {grupoActualId && grupos && !estaEnLista && (
              <option value={grupoActualId}>Grupo cerrado</option>
            )}
            {grupos?.map((g) => (
              <option key={g.id} value={g.id}>
                {g.descripcion}
              </option>
            ))}
          </select>
        </label>

        <div className="topbar-acciones">
          <Link className="btn btn-green" to="/grupos/nuevo">
            Nuevo grupo
          </Link>
          <span className="quien" title={sesion.email}>
            {sesion.email}
          </span>
          <button type="button" className="btn btn-ghost" onClick={cerrarSesion}>
            Salir
          </button>
        </div>
      </header>

      <main className="contenido">
        <Outlet context={{ recargarGrupos }} />
      </main>
    </>
  );
}
