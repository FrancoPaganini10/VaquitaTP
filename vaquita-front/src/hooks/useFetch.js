import { useCallback, useEffect, useState } from "react";

// Carga datos al montar / cuando cambian las dependencias. Mientras recarga
// conserva los datos anteriores (evita parpadeos al refrescar).
export function useFetch(fn, deps = []) {
  const [estado, setEstado] = useState({ data: null, error: null, loading: true });
  const [tick, setTick] = useState(0);

  useEffect(() => {
    let cancelado = false;
    setEstado((s) => ({ ...s, loading: true, error: null }));
    fn()
      .then((data) => !cancelado && setEstado({ data, error: null, loading: false }))
      .catch((error) => !cancelado && setEstado({ data: null, error, loading: false }));
    return () => {
      cancelado = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...deps, tick]);

  const reload = useCallback(() => setTick((t) => t + 1), []);
  return { ...estado, reload };
}
