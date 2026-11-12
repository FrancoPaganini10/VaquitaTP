import { useEffect } from "react";
import { useLocation } from "react-router-dom";

export default function AeroScene() {
  const { pathname } = useLocation();

  useEffect(() => {
    if (pathname === "/login") {
      document.body.classList.add("fondo-login");
      document.body.classList.remove("fondo-app");
    } else {
      document.body.classList.add("fondo-app");
      document.body.classList.remove("fondo-login");
    }

    return () => {
      document.body.classList.remove("fondo-login", "fondo-app");
    };
  }, [pathname]);

  return null;
}