import { request } from "./client.js";

export const usuariosApi = {
  registrar: (dto) => request("/usuarios/registrar", { method: "POST", body: dto }),
  solicitarAcceso: (email) => request("/usuarios/acceso", { method: "POST", body: { email } }),
  verificar: (token) => request("/usuarios/verificar", { method: "POST", body: { token } }),
};

export const gruposApi = {
  listar: () => request("/grupos"), // solo activos
  ultimo: () => request("/grupos/ultimo"), // null si no tiene grupos (204)
  obtener: (id) => request(`/grupos/${id}`), // solo lectura: no modifica datos
  // Marca el grupo como el actual (204 sin body). Sin esto, al reabrir la app no vuelve a este grupo.
  seleccionar: (id) => request(`/grupos/${id}/seleccionar`, { method: "POST" }),
  crear: (dto) => request("/grupos", { method: "POST", body: dto }),
  // Datos públicos del grupo ({ descripcion }) para "¿Querés unirte a X?". Con token usa la invitación; si no, el código.
  publico: ({ codigo, token }) => request("/grupos/publico", { query: token ? { token } : { codigo } }),
  
  unirse: (codigo, token) =>
  request("/grupos/unirse", {
    method: "POST",
    body: {
      codigo: codigo || null,
      tokenInvitacion: token || null
    }
  }),

  invitar: (id, email) => request(`/grupos/${id}/invitaciones`, { method: "POST", body: { email } }),
  qr: (id) => request(`/grupos/${id}/qr`, { blob: true }),
  cerrar: (id) => request(`/grupos/${id}/cerrar`, { method: "POST" }),
};

export const gastosApi = {
  listar: (grupoId) => request(`/grupos/${grupoId}/gastos`),
  porRubro: (grupoId) => request(`/grupos/${grupoId}/gastos/por-rubro`),
  crear: (grupoId, dto) => request(`/grupos/${grupoId}/gastos`, { method: "POST", body: dto }),
  anular: (grupoId, gastoId) => request(`/grupos/${grupoId}/gastos/${gastoId}`, { method: "DELETE" }),
};

export const pagosApi = {
  listar: (grupoId) => request(`/grupos/${grupoId}/pagos`),
  crear: (grupoId, dto) => request(`/grupos/${grupoId}/pagos`, { method: "POST", body: dto }),
  anular: (grupoId, pagoId) => request(`/grupos/${grupoId}/pagos/${pagoId}`, { method: "DELETE" }),
};

export const saldosApi = {
  obtener: (grupoId) => request(`/grupos/${grupoId}/saldos`),
};

export const rubrosApi = {
  listar: () => request("/rubros"),
};