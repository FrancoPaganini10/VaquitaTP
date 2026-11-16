import { useState } from "react";
import { Link } from "react-router-dom";
import { usuariosApi } from "../api/endpoints.js";
import Field from "../components/Field.jsx";
import { FormError } from "../components/Estado.jsx";
import { useForm } from "../hooks/useForm.js";
import { registroSchema } from "../lib/schemas.js";
import AuthShell from "./AuthShell.jsx";

export default function Registro() {
  const [enviadoA, setEnviadoA] = useState("");
  const form = useForm({
    schema: registroSchema,
    initial: { apellido: "", nombre: "", apodo: "", email: "" },
    onSubmit: async (datos) => {
      await usuariosApi.registrar(datos);
      setEnviadoA(datos.email);
    },
  });

  if (enviadoA) {
    return (
      <AuthShell titulo="¡Cuenta creada!">
        <p>
          Te mandamos un enlace a <strong>{enviadoA}</strong>. Al abrirlo desde este dispositivo ingresás a Vaquita y
          te queda la sesión iniciada.
        </p>
        {import.meta.env.DEV && (
          <p className="hint">
            Modo desarrollo: sin SMTP configurado, el enlace aparece en la consola del backend.
          </p>
        )}
      </AuthShell>
    );
  }

  return (
    <AuthShell titulo="Creá tu cuenta">
      <form onSubmit={form.handleSubmit} noValidate>
        <div className="dos-col">
          <Field label="Apellido" autoComplete="family-name" {...form.bind("apellido")} />
          <Field label="Nombre" autoComplete="given-name" {...form.bind("nombre")} />
        </div>
        <Field
          label="Apodo"
          hint="Así te van a ver los demás en los gastos y saldos."
          autoComplete="nickname"
          {...form.bind("apodo")}
        />
        <Field label="Email" type="email" autoComplete="email" {...form.bind("email")} />
        <FormError mensaje={form.formError} />
        <button className="btn" disabled={form.submitting}>
          {form.submitting ? "Creando cuenta…" : "Crear cuenta"}
        </button>
      </form>
      <p className="aparte">
        ¿Ya tenés cuenta? <Link to="/login">Ingresá</Link>
      </p>
    </AuthShell>
  );
}
