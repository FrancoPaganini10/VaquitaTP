import { useState } from "react";
import { ApiError } from "../api/client.js";

// Formularios controlados validados con un schema de zod.
// onSubmit recibe los datos YA convertidos por zod (números, trim, etc.).
export function useForm({ schema, initial, onSubmit }) {
  const [values, setValues] = useState(initial);
  const [errors, setErrors] = useState({});
  const [formError, setFormError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const setField = (name, value) => {
    setValues((v) => ({ ...v, [name]: value }));
    setErrors((e) => ({ ...e, [name]: undefined }));
  };

  // Se esparce sobre <Field {...form.bind("email")} />
  const bind = (name) => ({
    name,
    value: values[name],
    error: errors[name],
    onChange: (e) => setField(name, e.target.value),
  });

  const reset = () => {
    setValues(initial);
    setErrors({});
    setFormError("");
  };

  async function handleSubmit(e) {
    e.preventDefault();
    setFormError("");

    const resultado = schema.safeParse(values);
    if (!resultado.success) {
      const porCampo = {};
      for (const issue of resultado.error.issues) {
        const campo = issue.path[0];
        if (campo !== undefined && !porCampo[campo]) porCampo[campo] = issue.message;
      }
      setErrors(porCampo);
      return;
    }

    setSubmitting(true);
    try {
      await onSubmit(resultado.data);
    } catch (err) {
      if (err instanceof ApiError) {
        setFormError(err.message);
        if (err.fieldErrors) setErrors(err.fieldErrors);
      } else {
        setFormError("Ocurrió un error inesperado. Probá de nuevo.");
      }
    } finally {
      setSubmitting(false);
    }
  }

  return { values, errors, formError, submitting, bind, reset, handleSubmit };
}
