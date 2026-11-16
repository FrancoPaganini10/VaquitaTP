import { request } from "./client.js";

export const usuariosApi = {
  registrar: (dto) => request("/usuarios/registrar", { method: "POST", body: dto }),
  solicitarAcceso: (email) => request("/usuarios/acceso", { method: "POST", body: { email } }),
  verificar: (token) => request("/usuarios/verificar", { method: "POST", body: { token } }),
};

export const gruposApi = {
  listar: () => request("/grupos"), // solo activos
  ultimo: () => request("/grupos/ultimo"), // null si no tiene grupos (204)
  obtener: (id) => request(`/grupos/${id}`),
  crear: (dto) => request("/grupos", { method: "POST", body: dto }),
  unirse: (id, token) =>
    request(`/grupos/${id}/unirse`, { method: "POST", query: token ? { token } : undefined }),
  invitar: (id, email) => request(`/grupos/${id}/invitaciones`, { method: "POST", body: { email } }),
  qr: (id) => request(`/grupos/${id}/qr`, { blob: true }),
  cerrar: (id) => request(`/grupos/${id}/cerrar`, { method: "POST" }),
};

export const gastosApi = {
  listar: (grupoId) => request(`/grupos/${grupoId}/gastos`),
  porRubro: (grupoId) => request(`/grupos/${grupoId}/gastos/por-rubro`),
  crear: (grupoId, dto) => request(`/grupos/${grupoId}/gastos`, { method: "POST", body: dto }),
};

export const pagosApi = {
  listar: (grupoId) => request(`/grupos/${grupoId}/pagos`),
  crear: (grupoId, dto) => request(`/grupos/${grupoId}/pagos`, { method: "POST", body: dto }),
};

export const saldosApi = {
  obtener: (grupoId) => request(`/grupos/${grupoId}/saldos`),
};

export const rubrosApi = {
  listar: () => request("/rubros"),
};
