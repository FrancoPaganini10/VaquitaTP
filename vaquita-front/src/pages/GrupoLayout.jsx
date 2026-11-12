import { useState } from "react";
import { NavLink, Outlet, useOutletContext, useParams } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import { useAuth } from "../auth/AuthContext.jsx";
import { Cargando, ErrorBanner } from "../components/Estado.jsx";
import InvitarModal from "../components/InvitarModal.jsx";
import { useFetch } from "../hooks/useFetch.js";
import { fecha } from "../lib/format.js";

function GrupoLayout({ grupoId }) {
  const padre = useOutletContext();
  const { sesion } = useAuth();
  const { data: grupo, error, loading, reload } = useFetch(() => gruposApi.obtener(grupoId), [grupoId]);
  const [sumando, setSumando] = useState(false);

  if (loading && !grupo) return <Cargando />;
  if (error) return <ErrorBanner error={error} volver />;

  const soyAdmin = grupo.administradorId === sesion.userId;

  return (
    <>
      <section className="glass panel grupo-head">
        <div>
          <h1>{grupo.descripcion}</h1>
          <p className="hint">
            {grupo.activo ? "Grupo activo" : `Grupo cerrado el ${fecha(grupo.fechaCierre)}`}
          </p>
        </div>
        <ul className="chips" aria-label="Integrantes">
          {grupo.integrantes.map((i) => (
            <li key={i.usuarioId} className="chip" title={`${i.nombre} ${i.apellido}`}>
              {i.apodo}
              {i.esAdministrador && <small> admin</small>}
            </li>
          ))}
        </ul>
        {grupo.activo && (
          <button type="button" className="btn" onClick={() => setSumando(true)}>
            Sumar integrantes
          </button>
        )}
      </section>

      <nav className="tabs" aria-label="Vistas del grupo">
        <NavLink to="gastos">Gastos</NavLink>
        <NavLink to="saldos">Saldos</NavLink>
      </nav>

      <Outlet context={{ ...padre, grupo, soyAdmin, miId: sesion.userId, recargarGrupo: reload }} />

      <InvitarModal open={sumando} onClose={() => setSumando(false)} grupo={grupo} soyAdmin={soyAdmin} />
    </>
  );
}

// key = grupoId: al cambiar de grupo se descarta todo el estado del anterior
export default function GrupoPorId() {
  const { grupoId } = useParams();
  return <GrupoLayout key={grupoId} grupoId={grupoId} />;
}
