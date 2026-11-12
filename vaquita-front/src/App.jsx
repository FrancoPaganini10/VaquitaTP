import { Navigate, Route, Routes } from "react-router-dom";
import { Privada, SoloVisitantes } from "./auth/Rutas.jsx";
import AeroScene from "./components/AeroScene.jsx";
import Layout from "./components/Layout.jsx";
import GrupoGastos from "./pages/GrupoGastos.jsx";
import GrupoPorId from "./pages/GrupoLayout.jsx";
import GrupoSaldos from "./pages/GrupoSaldos.jsx";
import Inicio from "./pages/Inicio.jsx";
import Login from "./pages/Login.jsx";
import NoEncontrado from "./pages/NoEncontrado.jsx";
import NuevoGrupo from "./pages/NuevoGrupo.jsx";
import Registro from "./pages/Registro.jsx";
import Unirse from "./pages/Unirse.jsx";
import Verificar from "./pages/Verificar.jsx";

export default function App() {
  return (
    <>
      <AeroScene />
      <Routes>
        <Route path="/login" element={<SoloVisitantes><Login /></SoloVisitantes>} />
        <Route path="/registro" element={<SoloVisitantes><Registro /></SoloVisitantes>} />
        <Route path="/verificar" element={<Verificar />} />
        <Route path="/unirse/:grupoId" element={<Privada><Unirse /></Privada>} />

        <Route element={<Privada><Layout /></Privada>}>
          <Route index element={<Inicio />} />
          <Route path="grupos/nuevo" element={<NuevoGrupo />} />
          <Route path="grupos/:grupoId" element={<GrupoPorId />}>
            <Route index element={<Navigate to="gastos" replace />} />
            <Route path="gastos" element={<GrupoGastos />} />
            <Route path="saldos" element={<GrupoSaldos />} />
          </Route>
        </Route>

        <Route path="*" element={<NoEncontrado />} />
      </Routes>
    </>
  );
}
