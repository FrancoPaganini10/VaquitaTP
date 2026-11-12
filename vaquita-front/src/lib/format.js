const numero = new Intl.NumberFormat("es-AR", {
  minimumFractionDigits: 0,
  maximumFractionDigits: 2,
  useGrouping: "always",
});

// Igual que en la consigna: 105.000$
export const moneda = (n) => `${numero.format(n)}$`;

// Cuadro 1: +46.650$ / -25.950$
export const saldoConSigno = (n) => {
  if (n > 0) return `+${moneda(n)}`;
  if (n < 0) return `-${moneda(Math.abs(n))}`;
  return moneda(0);
};

export const fecha = (iso) => new Date(iso).toLocaleDateString("es-AR");

// Acepta "1.234,56", "1234,56", "1234.56" y "$ 1.000". Devuelve null si no es un número.
export function parseImporte(texto) {
  const limpio = texto.replace(/[\s$]/g, "");
  let normal;
  if (limpio.includes(",")) {
    normal = limpio.replace(/\./g, "").replace(",", ".");
  } else if (/^\d{1,3}(\.\d{3})+$/.test(limpio)) {
    normal = limpio.replace(/\./g, "");
  } else {
    normal = limpio;
  }
  return /^\d+(\.\d+)?$/.test(normal) ? Number(normal) : null;
}

// Para precargar un input con un número del backend
export const importeATexto = (n) => String(n).replace(".", ",");

// Decodifica el payload del JWT (sin verificar: eso lo hace el backend)
export function parseJwt(token) {
  try {
    const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
        .join("")
    );
    return JSON.parse(json);
  } catch {
    return null;
  }
}
