import { useEffect } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { REDIRECT_KEY, useAuth } from "./AuthContext.jsx";

// Exige sesión. Si el usuario llega desde un QR/invitación sin estar logueado,
// guardamos el destino para volver después de validar el link del mail.
export function Privada({ children }) {
  const { sesion } = useAuth();
  const { pathname, search } = useLocation();
  const guardarDestino = !sesion && pathname.startsWith("/unirse");

  useEffect(() => {
    if (guardarDestino) localStorage.setItem(REDIRECT_KEY, pathname + search);
  }, [guardarDestino, pathname, search]);

  return sesion ? children : <Navigate to="/login" replace />;
}

// Login y registro: si ya hay sesión no tienen sentido
export function SoloVisitantes({ children }) {
  const { sesion } = useAuth();
  return sesion ? <Navigate to="/" replace /> : children;
}
