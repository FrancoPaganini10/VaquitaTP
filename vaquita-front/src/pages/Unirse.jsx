import { useState } from "react";
import { Link, useNavigate, useParams, useSearchParams } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import { Cargando, FormError } from "../components/Estado.jsx";
import { useFetch } from "../hooks/useFetch.js";
import AuthShell from "./AuthShell.jsx";

// Destino de la invitación por mail: /unirse/:idGrupo?token=...
// Primero muestra "¿Querés unirte a X?" (GET /grupos/publico) y recién se une al confirmar.
export default function Unirse() {
  const { codigo } = useParams();
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const token = params.get("token");
  const incompleto = !codigo || !token;

  const { data: grupo, error: errorGrupo, loading } = useFetch(
    () => (incompleto ? Promise.resolve(null) : gruposApi.publico({ codigo, token })),
    [codigo, token, incompleto]
  );
  const [uniendo, setUniendo] = useState(false);
  const [errorUnirse, setErrorUnirse] = useState("");

  async function unirse() {
    setUniendo(true);
    setErrorUnirse("");
    try {
      await gruposApi.unirse(codigo, token);
      navigate("/", { replace: true });
    } catch (e) {
      setErrorUnirse(e.message);
      setUniendo(false);
    }
  }

  const error = incompleto ? "El enlace de invitación está incompleto." : errorGrupo?.message;
  if (error) {
    return (
      <AuthShell titulo="No pudimos sumarte al grupo">
        <FormError mensaje={error} />
        <Link className="btn" to="/">
          Ir al inicio
        </Link>
      </AuthShell>
    );
  }

  if (loading || !grupo) {
    return (
      <AuthShell titulo="Invitación a un grupo">
        <Cargando>Un momento…</Cargando>
      </AuthShell>
    );
  }

  return (
    <AuthShell titulo="Invitación a un grupo">
      <p>
        ¿Querés unirte a <strong>{grupo.descripcion}</strong>?
      </p>
      <FormError mensaje={errorUnirse} />
      <button type="button" className="btn btn-green" onClick={unirse} disabled={uniendo}>
        {uniendo ? "Uniéndote…" : "Unirme al grupo"}
      </button>
      <Link className="btn btn-ghost" to="/">
        Ahora no
      </Link>
    </AuthShell>
  );
}
