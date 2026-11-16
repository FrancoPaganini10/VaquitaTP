import { useEffect } from "react";
import { useLocation } from "react-router-dom";

// El fondo por defecto es el de la app (ver aero.css). Solo /login lleva su propia imagen.
export default function AeroScene() {
  const { pathname } = useLocation();
  const esLogin = pathname === "/login";

  useEffect(() => {
    document.body.classList.toggle("fondo-login", esLogin);
    return () => document.body.classList.remove("fondo-login");
  }, [esLogin]);

  return null;
}
