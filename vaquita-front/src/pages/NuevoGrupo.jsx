import { useNavigate, useOutletContext } from "react-router-dom";
import { gruposApi } from "../api/endpoints.js";
import Field from "../components/Field.jsx";
import { FormError } from "../components/Estado.jsx";
import { useForm } from "../hooks/useForm.js";
import { grupoSchema } from "../lib/schemas.js";

export default function NuevoGrupo() {
  const { recargarGrupos } = useOutletContext();
  const navigate = useNavigate();
  const form = useForm({
    schema: grupoSchema,
    initial: { descripcion: "" },
    onSubmit: async (datos) => {
      const grupo = await gruposApi.crear(datos);
      recargarGrupos();
      navigate(`/grupos/${grupo.id}/gastos`);
    },
  });

  return (
    <section className="glass panel angosto">
      <h1>Nuevo grupo</h1>
      <p className="hint">Vos quedás como administrador. Después sumás a los demás con un QR o por mail.</p>
      <form onSubmit={form.handleSubmit} noValidate>
        <Field label="Descripción" placeholder="Ej: Mendoza 2026" {...form.bind("descripcion")} />
        <FormError mensaje={form.formError} />
        <button className="btn btn-green" disabled={form.submitting}>
          {form.submitting ? "Creando…" : "Crear grupo"}
        </button>
      </form>
    </section>
  );
}
