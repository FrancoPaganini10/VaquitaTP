import { useState } from "react";
import { Link } from "react-router-dom";
import { usuariosApi } from "../api/endpoints.js";
import EnlaceEnviado from "../components/EnlaceEnviado.jsx";
import Field from "../components/Field.jsx";
import { FormError } from "../components/Estado.jsx";
import { useForm } from "../hooks/useForm.js";
import { loginSchema } from "../lib/schemas.js";
import AuthShell from "./AuthShell.jsx";

export default function Login() {
  const [enviadoA, setEnviadoA] = useState("");
  const form = useForm({
    schema: loginSchema,
    initial: { email: "" },
    onSubmit: async ({ email }) => {
      await usuariosApi.solicitarAcceso(email);
      setEnviadoA(email);
    },
  });

  if (enviadoA) {
    return <EnlaceEnviado email={enviadoA} onOtroEmail={() => setEnviadoA("")} />;
  }

  return (
    <AuthShell titulo="Ingresá a tu cuenta">
      <form onSubmit={form.handleSubmit} noValidate>
        <Field label="Email" type="email" autoComplete="email" {...form.bind("email")} />
        <FormError mensaje={form.formError} />
        <button className="btn" disabled={form.submitting}>
          {form.submitting ? "Enviando…" : "Enviarme el enlace de acceso"}
        </button>
      </form>
      <p className="aparte">
        ¿Todavía no tenés cuenta? <Link to="/registro">Registrate</Link>
      </p>
    </AuthShell>
  );
}
