
import { useEffect, useRef, useState } from "react";
import { Link, useNavigate, useSearchParams, useParams } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import { Cargando, FormError } from "../components/Estado.jsx";
import AuthShell from "./AuthShell.jsx";

export default function Unirse() {
  const { codigo } = useParams();
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const yaLlamado = useRef(false);

  const token = params.get("token");

  useEffect(() => {
    if (yaLlamado.current) return;
    yaLlamado.current = true;

    if (!codigo || !token) {
      setError("El enlace de invitación está incompleto.");
      return;
    }

    gruposApi
      .unirse(codigo, token)
      .then(() => navigate("/", { replace: true }))
      .catch((e) => setError(e.message));
  }, [codigo, token, navigate]);

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

  return (
    <AuthShell titulo="Sumándote al grupo…">
      <Cargando>Un momento…</Cargando>
    </AuthShell>
  );
}
