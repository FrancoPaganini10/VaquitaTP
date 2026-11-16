import { test } from "node:test";
import assert from "node:assert/strict";
import { gastoSchema, crearPagoSchema, registroSchema } from "./schemas.js";
import { parseImporte, moneda, saldoConSigno } from "./format.js";

test("parseImporte entiende formatos argentinos", () => {
  assert.equal(parseImporte("1.234,56"), 1234.56);
  assert.equal(parseImporte("1234,5"), 1234.5);
  assert.equal(parseImporte("1.000"), 1000);
  assert.equal(parseImporte("12.5"), 12.5);
  assert.equal(parseImporte("$ 25.950"), 25950);
  assert.equal(parseImporte("abc"), null);
});

test("formato igual al de la consigna", () => {
  assert.equal(moneda(105000), "105.000$");
  assert.equal(moneda(1234), "1.234$");
  assert.equal(saldoConSigno(46650), "+46.650$");
  assert.equal(saldoConSigno(-25950), "-25.950$");
});

test("gasto válido: convierte importe, rubro y pagador", () => {
  const r = gastoSchema.safeParse({ importe: "1.500,50", descripcion: "  Cena viernes ", rubroId: "", pagadorId: "3" });
  assert.equal(r.success, true);
  assert.deepEqual(r.data, { importe: 1500.5, descripcion: "Cena viernes", rubroId: null, pagadorId: 3 });
});

test("gasto inválido: importe 0, 3 decimales, texto, descripción vacía", () => {
  const base = { descripcion: "x", rubroId: "", pagadorId: "1" };
  for (const importe of ["0", "10,999", "hola", ""]) {
    assert.equal(gastoSchema.safeParse({ ...base, importe }).success, false, importe);
  }
  const r = gastoSchema.safeParse({ ...base, importe: "10", descripcion: "   " });
  assert.equal(r.success, false);
  assert.equal(r.error.issues[0].path[0], "descripcion");
});

test("pago parcial ok, pago mayor a la deuda no", () => {
  const schema = crearPagoSchema(20700);
  assert.equal(schema.safeParse({ importe: "10.000" }).success, true);
  assert.equal(schema.safeParse({ importe: "20.700" }).success, true);
  assert.equal(schema.safeParse({ importe: "20.700,01" }).success, false);
});

test("registro exige los 4 campos y email válido", () => {
  assert.equal(registroSchema.safeParse({ apellido: "Paz", nombre: "Ana", apodo: "Ani", email: "ana@x.com" }).success, true);
  assert.equal(registroSchema.safeParse({ apellido: "", nombre: "Ana", apodo: "Ani", email: "no-es-mail" }).success, false);
});
