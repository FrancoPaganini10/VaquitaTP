import { Link } from "react-router-dom";
import AuthShell from "./AuthShell.jsx";

export default function NoEncontrado() {
  return (
    <AuthShell titulo="Esta página no existe">
      <p>El enlace que seguiste no lleva a ningún lado.</p>
      <Link className="btn" to="/">
        Ir al inicio
      </Link>
    </AuthShell>
  );
}
