import { useState } from "react";
import { FormError } from "./Estado.jsx";
import Modal from "./Modal.jsx";

// Confirmación para anular un pago o un gasto. `item` null = cerrado.
function AnularContenido({ item, descripcion, onAnular, onClose, onAnulado }) {
  const [error, setError] = useState("");
  const [anulando, setAnulando] = useState(false);

  async function anular() {
    setAnulando(true);
    setError("");
    try {
      await onAnular(item);
      onClose();
      onAnulado();
    } catch (e) {
      setError(e.message);
      setAnulando(false);
    }
  }

  return (
    <>
      <p>{descripcion(item)}</p>
      <FormError mensaje={error} />
      <div className="acciones">
        <button type="button" className="btn btn-ghost" onClick={onClose}>
          Cancelar
        </button>
        <button type="button" className="btn" onClick={anular} disabled={anulando}>
          {anulando ? "Anulando…" : "Anular"}
        </button>
      </div>
    </>
  );
}

export default function AnularModal({ item, titulo, ...resto }) {
  return (
    <Modal open={!!item} onClose={resto.onClose} titulo={titulo}>
      {item && <AnularContenido item={item} {...resto} />}
    </Modal>
  );
}