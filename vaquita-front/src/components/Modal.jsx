import { useEffect, useRef } from "react";

// <dialog> nativo: maneja foco, Esc y fondo bloqueado.
export default function Modal({ open, onClose, titulo, children }) {
  const ref = useRef(null);

  useEffect(() => {
    const dialog = ref.current;
    if (!dialog) return;
    if (open && !dialog.open) {
      dialog.showModal();
      dialog.querySelector("[data-autofocus]")?.focus();
    }
    if (!open && dialog.open) dialog.close();
  }, [open]);

  return (
    <dialog
      ref={ref}
      className="aero-modal"
      onClose={onClose}
      onClick={(e) => e.target === ref.current && onClose()}
      aria-label={titulo}
    >
      {open && (
        <div className="glass modal-body">
          <header className="modal-head">
            <h2>{titulo}</h2>
            <button type="button" className="btn-cerrar" onClick={onClose} aria-label="Cerrar">
              ×
            </button>
          </header>
          {children}
        </div>
      )}
    </dialog>
  );
}
