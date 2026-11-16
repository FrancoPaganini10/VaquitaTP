import { useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams, useSearchParams } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import AuthShell from "./AuthShell.jsx";

// Destino común del QR y del link de invitación por mail: /unirse/:grupoId[?token=...]
export default function Unirse() {
  const { grupoId } = useParams();
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const yaLlamado = useRef(false);

  useEffect(() => {
    if (yaLlamado.current) return;
    yaLlamado.current = true;
    gruposApi
      .unirse(grupoId, params.get("token"))
      .then(() => navigate(`/grupos/${grupoId}/gastos`, { replace: true }))
      .catch((e) => setError(e.message));
  }, [grupoId, params, navigate]);

  if (error) {
    return (
      <AuthShell titulo="No pudimos sumarte al grupo">
        <p className="banner banner-error" role="alert">
          {error}
        </p>
        <Link className="btn" to="/">
          Ir al inicio
        </Link>
      </AuthShell>
    );
  }
  return (
    <AuthShell titulo="Sumándote al grupo…">
      <p role="status" className="cargando">
        Un momento…
      </p>
    </AuthShell>
  );
}
