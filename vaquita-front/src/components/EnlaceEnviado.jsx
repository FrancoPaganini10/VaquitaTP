import { useEffect, useState } from "react";
import { usuariosApi } from "../api/endpoints.js";
import AuthShell from "../pages/AuthShell.jsx";
import AvisoDev from "./AvisoDev.jsx";
import { FormError } from "./Estado.jsx";

const ESPERA_SEGUNDOS = 30;

// Pantalla posterior a registro / pedido de acceso.
// El back responde 200 igual exista o no el email (a propósito) y no garantiza la entrega del mail,
// así que NUNCA afirmamos "enviado a X" ni "ese email no existe": solo "si es correcto, vas a recibir...".
// "Reenviar enlace" llama a POST /usuarios/acceso, que sirve tanto para registrados como para quien quiere loguearse.
export default function EnlaceEnviado({ email, onOtroEmail }) {
  const [reenviando, setReenviando] = useState(false);
  const [reenviado, setReenviado] = useState(false);
  const [error, setError] = useState("");
  const [espera, setEspera] = useState(ESPERA_SEGUNDOS); // evita spam de reenvíos

  useEffect(() => {
    if (espera <= 0) return;
    const id = setTimeout(() => setEspera((s) => s - 1), 1000);
    return () => clearTimeout(id);
  }, [espera]);

  async function reenviar() {
    setReenviando(true);
    setError("");
    setReenviado(false);
    try {
      await usuariosApi.solicitarAcceso(email);
      setReenviado(true);
      setEspera(ESPERA_SEGUNDOS);
    } catch (e) {
      setError(e.message);
    } finally {
      setReenviando(false);
    }
  }

  return (
    <AuthShell titulo="Revisá tu correo">
      <p>
        Si el email es correcto, vas a recibir un enlace para ingresar. Abrilo desde este dispositivo y vas a
        quedar con la sesión iniciada.
      </p>
      <p className="hint">
        Si no te llega en unos minutos, revisá la carpeta de spam o pedí un nuevo enlace.
      </p>
      <p className="hint">
        Email ingresado: <strong>{email}</strong>
      </p>
      <AvisoDev />

      {reenviado && (
        <p className="banner banner-ok" role="status">
          Listo. Si el email es correcto, vas a recibir un nuevo enlace. El anterior puede dejar de funcionar.
        </p>
      )}
      <FormError mensaje={error} />

      <button type="button" className="btn" onClick={reenviar} disabled={reenviando || espera > 0}>
        {reenviando ? "Reenviando…" : espera > 0 ? `Reenviar enlace (${espera}s)` : "Reenviar enlace"}
      </button>
      <button type="button" className="btn btn-ghost" onClick={onOtroEmail}>
        Usar otro email
      </button>
    </AuthShell>
  );
}
