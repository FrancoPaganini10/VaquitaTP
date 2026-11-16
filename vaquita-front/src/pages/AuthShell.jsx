// Tarjeta centrada para login, registro y verificación
export default function AuthShell({ titulo, children }) {
  return (
    <main className="auth-wrap">
      <section className="glass auth-card">
        <p className="logo logo-grande">
          <span aria-hidden="true">🐮</span> Vaquita
        </p>
        <h1>{titulo}</h1>
        {children}
      </section>
    </main>
  );
}
