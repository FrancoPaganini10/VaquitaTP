# Vaquita · Front

React + react-router-dom + zod (Vite), estilo Frutiger Aero.

## Cómo correrlo

1. En el back, en `appsettings.Development.json`: `"FrontendUrl": "http://localhost:5173"`
   (los links de los mails y el QR apuntan ahí).
2. Copiá `.env.example` a `.env.local` y poné el puerto real de tu API en `VITE_PROXY_TARGET`
   (lo ves en `launchSettings.json` o en la consola al correr el back).
3. `npm install` y `npm run dev` → http://localhost:5173
4. Sin SMTP configurado, el back "envía" los mails a la consola: copiá el link de ahí.

`npm test` corre las pruebas de los schemas de zod y del formato de importes.

## Estructura

- `src/api/` cliente `fetch` (agrega el JWT, unifica los errores del back) y endpoints.
- `src/auth/` sesión (JWT en localStorage) y rutas privadas.
- `src/lib/schemas.js` todas las validaciones con zod (espejo de los DTO del back).
- `src/hooks/useForm.js` formularios validados con zod. `useFetch.js` carga de datos.
- `src/pages/` una por pantalla. `src/components/` piezas compartidas.
- `src/styles/aero.css` todo el estilo.

## Rutas

| Ruta | Pantalla |
|---|---|
| `/login`, `/registro` | Alta y acceso por link al mail |
| `/verificar?token=` | Destino del link del mail: valida el token y deja la sesión iniciada |
| `/unirse/:grupoId?token=` | Destino común del QR y de la invitación por mail |
| `/` | Abre el último grupo trabajado |
| `/grupos/nuevo` | Crear grupo |
| `/grupos/:id/gastos` | Gastos del grupo (usuario, detalle, importe) |
| `/grupos/:id/saldos` | Cuadro 1 y Cuadro 2 en la misma página, pagos y cierre del grupo |
