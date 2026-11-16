import { Link } from "react-router-dom";

export const Cargando = ({ children = "Cargando…" }) => (
  <p role="status" className="cargando">
    {children}
  </p>
);

export const ErrorBanner = ({ error, volver = false }) => (
  <div className="banner banner-error" role="alert">
    <p>{error?.message ?? "Ocurrió un error inesperado."}</p>
    {volver && (
      <Link className="btn btn-ghost" to="/">
        Volver al inicio
      </Link>
    )}
  </div>
);

export const FormError = ({ mensaje }) =>
  mensaje ? (
    <p className="banner banner-error" role="alert">
      {mensaje}
    </p>
  ) : null;
