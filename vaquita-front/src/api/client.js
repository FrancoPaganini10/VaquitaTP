export const TOKEN_KEY = "vaquita.token";

// Vacío = misma origen (en desarrollo, Vite reenvía /api al backend)
const BASE = import.meta.env.VITE_API_URL ?? "";

export class ApiError extends Error {
  constructor(status, message, fieldErrors = null) {
    super(message);
    this.status = status;
    this.fieldErrors = fieldErrors; // { campo: "mensaje" } cuando el back valida los DTO
  }
}

let alVencerSesion = () => {};
export const onSesionVencida = (fn) => {
  alVencerSesion = fn;
};

// El back responde siempre ProblemDetails: el mensaje está en `detail` y, en la validación
// de DTO, además viene `errors: { Campo: [..] }` con los mensajes por campo.
async function construirError(res) {
  let data = null;
  try {
    data = await res.json();
  } catch {
    /* sin cuerpo */
  }

  let fieldErrors = null;
  if (data?.errors && typeof data.errors === "object") {
    fieldErrors = {};
    for (const [campo, mensajes] of Object.entries(data.errors)) {
      const clave = campo.charAt(0).toLowerCase() + campo.slice(1);
      fieldErrors[clave] = Array.isArray(mensajes) ? mensajes[0] : String(mensajes);
    }
  }

  const mensaje =
    data?.detail ??
    (fieldErrors && Object.values(fieldErrors).join(" ")) ??
    data?.title ??
    (res.status === 403 ? "No tenés permiso para hacer esto." : `Error ${res.status}`);

  return new ApiError(res.status, mensaje, fieldErrors);
}

export async function request(path, { method = "GET", body, query, blob = false } = {}) {
  const headers = {};
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) headers.Authorization = `Bearer ${token}`;
  if (body !== undefined) headers["Content-Type"] = "application/json";

  const qs = query ? `?${new URLSearchParams(query)}` : "";

  let res;
  try {
    res = await fetch(`${BASE}/api${path}${qs}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });
  } catch {
    throw new ApiError(0, "No se pudo conectar con el servidor. Revisá tu conexión.");
  }

  if (res.status === 401) {
    if (token) alVencerSesion();
    throw new ApiError(401, "Tu sesión venció. Volvé a ingresar.");
  }
  if (!res.ok) {
    const error = await construirError(res);
    // Con el back caído, el proxy de Vite responde 500/502/503/504 sin cuerpo JSON de la API
    if ([500, 502, 503, 504].includes(res.status) && error.message === `Error ${res.status}`) {
      throw new ApiError(0, "No se pudo conectar con el servidor. Revisá que el back esté levantado.");
    }
    throw error;
  }
  if (res.status === 204) return null;
  if (blob) return res.blob();

  const texto = await res.text();
  return texto ? JSON.parse(texto) : null;
}