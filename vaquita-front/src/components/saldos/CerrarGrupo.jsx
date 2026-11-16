import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { gruposApi } from "../../api/endpoints.js";
import { FormError } from "../Estado.jsx";
import Modal from "../Modal.jsx";

// Solo el administrador puede cerrar, y únicamente cuando todos los saldos están en cero.
export default function CerrarGrupo({ grupo, saldado, soyAdmin, recargarGrupos }) {
  const navigate = useNavigate();
  const [confirmando, setConfirmando] = useState(false);
  const [error, setError] = useState("");
  const [cerrando, setCerrando] = useState(false);

  async function cerrar() {
    setCerrando(true);
    setError("");
    try {
      await gruposApi.cerrar(grupo.id);
      recargarGrupos();
      navigate("/");
    } catch (e) {
      setError(e.message);
      setCerrando(false);
    }
  }

  let motivo = "";
  if (!soyAdmin) motivo = "Solo el administrador puede cerrar el grupo.";
  else if (!saldado) motivo = "Para cerrar el grupo, todos los saldos tienen que estar en cero.";

  return (
    <section className="glass panel cerrar">
      <div>
        <h2>Cerrar grupo</h2>
        <p className={motivo ? "hint" : "ok-texto"}>
          {motivo || "Todos los saldos están en cero. Ya podés cerrar el grupo."}
        </p>
      </div>
      <button type="button" className="btn btn-ghost" disabled={Boolean(motivo)} onClick={() => setConfirmando(true)}>
        Cerrar grupo
      </button>

      <Modal open={confirmando} onClose={() => setConfirmando(false)} titulo="¿Cerrar el grupo?">
        <p>
          Al cerrar <strong>{grupo.descripcion}</strong> ya no vas a poder cargar gastos ni pagos, y deja de aparecer
          en la lista de grupos activos.
        </p>
        <FormError mensaje={error} />
        <div className="acciones">
          <button type="button" className="btn btn-ghost" onClick={() => setConfirmando(false)}>
            Cancelar
          </button>
          <button type="button" className="btn" onClick={cerrar} disabled={cerrando}>
            {cerrando ? "Cerrando…" : "Cerrar grupo"}
          </button>
        </div>
      </Modal>
    </section>
  );
}
