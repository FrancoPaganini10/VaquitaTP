import { useMemo, useState } from "react";
import { pagosApi } from "../api/endpoints.js";
import { useForm } from "../hooks/useForm.js";
import { importeATexto, moneda } from "../lib/format.js";
import { crearPagoSchema } from "../lib/schemas.js";
import Field from "./Field.jsx";
import Modal from "./Modal.jsx";
import { FormError } from "./Estado.jsx";

const centavos = (n) => Math.round(n * 100) / 100;

// Formulario del importe. Se remonta (key) al cambiar de acreedor para recargar el máximo.
function PagoImporteForm({ grupoId, deuda, acreedorId, maximo, inicial, onGuardado }) {
  const schema = useMemo(() => crearPagoSchema(maximo), [maximo]);
  const form = useForm({
    schema,
    initial: { importe: importeATexto(inicial) },
    onSubmit: async ({ importe }) => {
      await pagosApi.crear(grupoId, {
        deudorId: deuda.deudorId,
        acreedorId,
        importe,
      });
      onGuardado();
    },
  });

  return (
    <form onSubmit={form.handleSubmit} noValidate>
      <Field
        label="Importe a pagar"
        inputMode="decimal"
        hint={`Máximo permitido: ${moneda(maximo)}. Podés pagar solo una parte; el resto queda pendiente.`}
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

function PagoForm({ grupoId, deuda, saldos, onGuardado }) {
  const [acreedorId, setAcreedorId] = useState(deuda.acreedorId);

  // Cualquier integrante al que le deban plata (saldo > 0) puede cobrar, no solo el sugerido.
  const acreedores = saldos.filter((s) => s.saldo > 0);
  const saldoDeudor = saldos.find((s) => s.usuarioId === deuda.deudorId)?.saldo ?? 0;
  const acreedor = acreedores.find((s) => s.usuarioId === acreedorId);

  const maximo = acreedor
    ? centavos(Math.min(-saldoDeudor, acreedor.saldo))
    : deuda.importe;
  // Con el acreedor sugerido se precarga la deuda del Cuadro 2; con otro, el máximo posible.
  const inicial = acreedorId === deuda.acreedorId ? deuda.importe : maximo;
  const nombreAcreedor = acreedor?.apodo ?? deuda.acreedor;

  return (
    <>
      <p>
        <strong>{deuda.deudor}</strong> le paga a <strong>{nombreAcreedor}</strong>.
      </p>
      {acreedores.length > 1 && (
        <Field
          as="select"
          label="Acreedor"
          name="acreedorId"
          value={acreedorId}
          onChange={(e) => setAcreedorId(Number(e.target.value))}
        >
          {acreedores.map((a) => (
            <option key={a.usuarioId} value={a.usuarioId}>
              {a.apodo} (le deben {moneda(a.saldo)})
            </option>
          ))}
        </Field>
      )}
      <PagoImporteForm
        key={acreedorId}
        grupoId={grupoId}
        deuda={deuda}
        acreedorId={acreedorId}
        maximo={maximo}
        inicial={inicial}
        onGuardado={onGuardado}
      />
    </>
  );
}

export default function PagoModal({ deuda, saldos, onClose, grupoId, onGuardado }) {
  return (
    <Modal open={!!deuda} onClose={onClose} titulo="Registrar pago">
      {deuda && (
        <PagoForm
          grupoId={grupoId}
          deuda={deuda}
          saldos={saldos}
          onGuardado={() => {
            onClose();
            onGuardado();
          }}
        />
      )}
    </Modal>
  );
}