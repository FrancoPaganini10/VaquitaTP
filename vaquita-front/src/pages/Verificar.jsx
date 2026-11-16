import { useEffect, useRef, useState } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { usuariosApi } from "../api/endpoints.js";
import { REDIRECT_KEY, useAuth } from "../auth/AuthContext.jsx";
import AuthShell from "./AuthShell.jsx";

// Destino del link del mail (alta y login): /verificar?token=...
export default function Verificar() {
  const [params] = useSearchParams();
  const token = params.get("token");
  const { iniciarSesion } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState(token ? "" : "El enlace está incompleto.");
  // El token es de un solo uso: evitamos llamarlo dos veces (StrictMode ejecuta los efectos 2 veces en desarrollo)
  const yaLlamado = useRef(false);

  useEffect(() => {
    if (!token || yaLlamado.current) return;
    yaLlamado.current = true;

    usuariosApi
      .verificar(token)
      .then(({ token: jwt }) => {
        iniciarSesion(jwt);
        const destino = localStorage.getItem(REDIRECT_KEY) ?? "/";
        localStorage.removeItem(REDIRECT_KEY);
        navigate(destino, { replace: true });
      })
      .catch((e) => setError(e.message));
  }, [token, iniciarSesion, navigate]);

  if (error) {
    return (
      <AuthShell titulo="No pudimos verificar el enlace">
        <p className="banner banner-error" role="alert">
          {error}
        </p>
        <Link className="btn" to="/login">
          Pedir un enlace nuevo
        </Link>
      </AuthShell>
    );
  }

  return (
    <AuthShell titulo="Verificando tu enlace…">
      <p role="status" className="cargando">
        Un momento, te estamos ingresando.
      </p>
    </AuthShell>
  );
}
