import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { TOKEN_KEY, onSesionVencida } from "../api/client.js";
import { parseJwt } from "../lib/format.js";

export const REDIRECT_KEY = "vaquita.redirect";

const AuthContext = createContext(null);

function leerSesion() {
  const token = localStorage.getItem(TOKEN_KEY);
  if (!token) return null;
  const claims = parseJwt(token);
  if (!claims || (claims.exp && claims.exp * 1000 < Date.now())) {
    localStorage.removeItem(TOKEN_KEY);
    return null;
  }
  return { userId: Number(claims.sub), email: claims.email };
}

export function AuthProvider({ children }) {
  const [sesion, setSesion] = useState(leerSesion);

  const iniciarSesion = useCallback((token) => {
    localStorage.setItem(TOKEN_KEY, token);
    setSesion(leerSesion());
  }, []);

  const cerrarSesion = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    setSesion(null);
  }, []);

  useEffect(() => onSesionVencida(cerrarSesion), [cerrarSesion]);

  const value = useMemo(
    () => ({ sesion, iniciarSesion, cerrarSesion }),
    [sesion, iniciarSesion, cerrarSesion]
  );
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);
