// Solo se muestra en desarrollo: sin SMTP el back "envía" el mail a su consola.
export default function AvisoDev() {
  if (!import.meta.env.DEV) return null;
  return <p className="hint">Modo desarrollo: sin SMTP configurado, el enlace aparece en la consola del backend.</p>;
}
