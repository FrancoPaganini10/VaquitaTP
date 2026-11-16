import { Navigate, useLocation } from "react-router-dom";
import { REDIRECT_KEY, useAuth } from "./AuthContext.jsx";

// Exige sesión. Si el usuario llega desde un QR/invitación sin estar logueado,
// guardamos el destino para volver después de validar el link del mail.
export function Privada({ children }) {
  const { sesion } = useAuth();
  const location = useLocation();

  if (!sesion) {
    if (location.pathname.startsWith("/unirse")) {
      localStorage.setItem(REDIRECT_KEY, location.pathname + location.search);
    }
    return <Navigate to="/login" replace />;
  }
  return children;
}

// Login y registro: si ya hay sesión no tienen sentido
export function SoloVisitantes({ children }) {
  const { sesion } = useAuth();
  return sesion ? <Navigate to="/" replace /> : children;
}
