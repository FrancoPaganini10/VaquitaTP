import { useId } from "react";

// Tarjeta de cristal con título: el <section> queda asociado a su <h2> para lectores de pantalla.
export default function Panel({ titulo, children }) {
  const id = useId();
  return (
    <section className="glass panel" aria-labelledby={id}>
      <h2 id={id}>{titulo}</h2>
      {children}
    </section>
  );
}
