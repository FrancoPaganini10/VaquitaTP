import { z } from "zod";
import { moneda, parseImporte } from "./format.js";

// Los límites replican los [StringLength]/[Range] de los DTO del backend.
const texto = (nombre, max) =>
  z
    .string()
    .trim()
    .min(1, `${nombre} es obligatorio`)
    .max(max, `${nombre} admite hasta ${max} caracteres`);

export const emailSchema = z
  .string()
  .trim()
  .min(1, "El email es obligatorio")
  .max(200, "El email admite hasta 200 caracteres")
  .email("Ingresá un email válido");

// El input es texto ("1.500,50"); el schema lo convierte a number y valida.
export const importeSchema = z
  .string()
  .trim()
  .min(1, "Ingresá un importe")
  .transform((valor, ctx) => {
    const n = parseImporte(valor);
    if (n === null) {
      ctx.addIssue({ code: "custom", message: "Importe inválido. Usá solo números, por ejemplo 1500,50" });
      return z.NEVER;
    }
    return n;
  })
  .pipe(
    z
      .number()
      .positive("El importe debe ser mayor a 0")
      .max(999999999.99, "El importe es demasiado alto")
      .refine((n) => Math.abs(n * 100 - Math.round(n * 100)) < 1e-6, "Máximo 2 decimales")
  );

export const loginSchema = z.object({ email: emailSchema });
export const invitacionSchema = z.object({ email: emailSchema });

export const registroSchema = z.object({
  apellido: texto("El apellido", 100),
  nombre: texto("El nombre", 100),
  apodo: texto("El apodo", 50),
  email: emailSchema,
});

export const grupoSchema = z.object({
  descripcion: texto("La descripción", 200),
});

export const gastoSchema = z.object({
  importe: importeSchema,
  descripcion: texto("La descripción", 200),
  rubroId: z.string().transform((v) => (v === "" ? null : Number(v))), // opcional
  pagadorId: z.string().min(1, "Elegí quién pagó").transform(Number),
});

// El pago no puede superar la deuda que muestra el Cuadro 2.
export const crearPagoSchema = (maximo) =>
  z.object({
    importe: importeSchema.refine(
      (n) => Math.round(n * 100) <= Math.round(maximo * 100),
      `No puede superar la deuda actual (${moneda(maximo)})`
    ),
  });
