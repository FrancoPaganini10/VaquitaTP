import { gastosApi, rubrosApi } from "../api/endpoints.js";
import { useFetch } from "../hooks/useFetch.js";
import { useForm } from "../hooks/useForm.js";
import { gastoSchema } from "../lib/schemas.js";
import Field from "./Field.jsx";
import Modal from "./Modal.jsx";
import { FormError } from "./Estado.jsx";

function GastoForm({ grupo, miId, onGuardado }) {
  const { data: rubros } = useFetch(() => rubrosApi.listar(), []);
  const form = useForm({
    schema: gastoSchema,
    initial: { importe: "", descripcion: "", rubroId: "", pagadorId: String(miId) },
    onSubmit: async (datos) => {
      await gastosApi.crear(grupo.id, datos);
      onGuardado();
    },
  });

  return (
    <form onSubmit={form.handleSubmit} noValidate>
      <Field label="Importe" inputMode="decimal" placeholder="0,00" data-autofocus {...form.bind("importe")} />
      <Field
        label="Descripción"
        placeholder="Ej: cena viernes, combustible YPF Potrerillo"
        {...form.bind("descripcion")}
      />
      <Field as="select" label="¿Quién pagó?" {...form.bind("pagadorId")}>
        {grupo.integrantes.map((i) => (
          <option key={i.usuarioId} value={i.usuarioId}>
            {i.apodo}
            {i.usuarioId === miId ? " (vos)" : ""}
          </option>
        ))}
      </Field>
      <Field
        as="select"
        label="Rubro (opcional)"
        hint="Sirve para saber cuánto gastó el grupo en cada cosa, por ejemplo combustible."
        {...form.bind("rubroId")}
      >
        <option value="">Sin rubro</option>
        {rubros?.map((r) => (
          <option key={r.id} value={r.id}>
            {r.nombre}
          </option>
        ))}
      </Field>
      <FormError mensaje={form.formError} />
      <button className="btn btn-green" disabled={form.submitting}>
        {form.submitting ? "Guardando…" : "Guardar gasto"}
      </button>
    </form>
  );
}

export default function GastoModal({ open, onClose, grupo, miId, onGuardado }) {
  return (
    <Modal open={open} onClose={onClose} titulo="Agregar gasto">
      <GastoForm
        grupo={grupo}
        miId={miId}
        onGuardado={() => {
          onClose();
          onGuardado();
        }}
      />
    </Modal>
  );
}
