import { useEffect, useState } from "react";
import { gruposApi } from "../api/endpoints.js";
import { useForm } from "../hooks/useForm.js";
import { invitacionSchema } from "../lib/schemas.js";
import Field from "./Field.jsx";
import Modal from "./Modal.jsx";
import { FormError } from "./Estado.jsx";

// El QR requiere Authorization, así que no puede ir en un <img src>: lo bajamos como blob.
function Qr({ grupoId }) {
  const [src, setSrc] = useState(null);
  const [fallo, setFallo] = useState(false);

  useEffect(() => {
    let url;
    let cancelado = false;
    gruposApi
      .qr(grupoId)
      .then((blob) => {
        if (cancelado) return;
        url = URL.createObjectURL(blob);
        setSrc(url);
      })
      .catch(() => !cancelado && setFallo(true));
    return () => {
      cancelado = true;
      if (url) URL.revokeObjectURL(url);
    };
  }, [grupoId]);

  if (fallo) return <p className="banner banner-error">No se pudo generar el código QR.</p>;
  if (!src) return <p role="status" className="cargando">Generando QR…</p>;
  return <img className="qr" src={src} alt="Código QR para unirse al grupo" width="220" height="220" />;
}

function InvitarPorMail({ grupoId }) {
  const [enviado, setEnviado] = useState("");
  const form = useForm({
    schema: invitacionSchema,
    initial: { email: "" },
    onSubmit: async ({ email }) => {
      await gruposApi.invitar(grupoId, email);
      setEnviado(email);
      form.reset();
    },
  });

  return (
    <form onSubmit={form.handleSubmit} noValidate>
      <Field label="Email del nuevo integrante" type="email" autoComplete="off" {...form.bind("email")} />
      <FormError mensaje={form.formError} />
      {enviado && (
        <p className="banner banner-ok" role="status">
          Invitación enviada a {enviado}.
        </p>
      )}
      <button className="btn" disabled={form.submitting}>
        {form.submitting ? "Enviando…" : "Enviar invitación"}
      </button>
    </form>
  );
}

export default function InvitarModal({ open, onClose, grupo, soyAdmin }) {
  return (
    <Modal open={open} onClose={onClose} titulo="Sumar integrantes">
      <section className="bloque">
        <h3>Con código QR</h3>
        <p className="hint">Los demás lo escanean con la cámara. Tienen que estar registrados.</p>
        <Qr grupoId={grupo.id} />
      </section>
      <section className="bloque">
        <h3>Por mail</h3>
        {soyAdmin ? (
          <InvitarPorMail grupoId={grupo.id} />
        ) : (
          <p className="hint">Solo el administrador del grupo puede invitar por mail.</p>
        )}
      </section>
    </Modal>
  );
}
