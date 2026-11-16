import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// En desarrollo, Vite reenvía /api al backend: así no hace falta CORS.
// Cambiá VITE_PROXY_TARGET (en .env.local) por la URL/puerto que use tu API.
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  return {
    plugins: [react()],
    server: {
      port: 5173,
      proxy: {
        "/api": {
          target: env.VITE_PROXY_TARGET || "http://localhost:5000",
          changeOrigin: true,
          secure: false, // certificado de desarrollo de .NET
        },
      },
    },
  };
});
