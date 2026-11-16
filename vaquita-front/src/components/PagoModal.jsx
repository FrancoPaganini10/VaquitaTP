import { useMemo } from "react";
import { pagosApi } from "../api/endpoints.js";
import { useForm } from "../hooks/useForm.js";
import { importeATexto, moneda } from "../lib/format.js";
import { crearPagoSchema } from "../lib/schemas.js";
import Field from "./Field.jsx";
import Modal from "./Modal.jsx";
import { FormError } from "./Estado.jsx";

function PagoForm({ grupoId, deuda, onGuardado }) {
  const schema = useMemo(() => crearPagoSchema(deuda.importe), [deuda.importe]);
  const form = useForm({
    schema,
    initial: { importe: importeATexto(deuda.importe) },
    onSubmit: async ({ importe }) => {
      await pagosApi.crear(grupoId, {
        deudorId: deuda.deudorId,
        acreedorId: deuda.acreedorId,
        importe,
      });
      onGuardado();
    },
  });

  return (
    <form onSubmit={form.handleSubmit} noValidate>
      <p>
        <strong>{deuda.deudor}</strong> le paga a <strong>{deuda.acreedor}</strong>. La deuda actual es{" "}
        <strong>{moneda(deuda.importe)}</strong>.
      </p>
      <Field
        label="Importe a pagar"
        inputMode="decimal"
        hint="Podés pagar solo una parte; el resto queda pendiente."
        data-autofocus
        {...form.bind("importe")}
      />
      <FormError mensaje={form.formError} />
      <button className="btn btn-green" disabled={form.submitting}>
        {form.submitting ? "Registrando…" : "Registrar pago"}
      </button>
    </form>
  );
}

export default function PagoModal({ deuda, onClose, grupoId, onGuardado }) {
  return (
    <Modal open={!!deuda} onClose={onClose} titulo="Registrar pago">
      {deuda && (
        <PagoForm
          grupoId={grupoId}
          deuda={deuda}
          onGuardado={() => {
            onClose();
            onGuardado();
          }}
        />
      )}
    </Modal>
  );
}
