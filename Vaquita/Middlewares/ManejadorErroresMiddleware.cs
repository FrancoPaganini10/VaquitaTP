using Microsoft.EntityFrameworkCore;
using System.Net;
using Vaquita.Excepciones;

namespace Vaquita.Middlewares
{
    // Todas las respuestas de error usan el mismo formato (application/problem+json):
    // { status, title, mensaje }. Es el mismo formato base que ProblemDetails de la validación
    // de DTOs ({ status, title, errors }), y "mensaje" es lo que ya lee el front.
    public class ManejadorErroresMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<ManejadorErroresMiddleware> logger;

        public ManejadorErroresMiddleware(RequestDelegate siguiente, ILogger<ManejadorErroresMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            try
            {
                await siguiente(contexto);
            }
            catch (ReglaDeNegocioException ex)
            {
                logger.LogInformation(ex, "Regla de negocio incumplida");
                await Responder(contexto, HttpStatusCode.BadRequest, "Solicitud inválida", ex.Message);
            }
            catch (NoEncontradoException ex)
            {
                logger.LogInformation(ex, "Recurso no encontrado");
                await Responder(contexto, HttpStatusCode.NotFound, "No encontrado", ex.Message);
            }
            catch (AccesoDenegadoException ex)
            {
                logger.LogInformation(ex, "Acceso denegado");
                await Responder(contexto, HttpStatusCode.Forbidden, "Acceso denegado", ex.Message);
            }
            catch (DbUpdateException ex)
            {
                // Por ejemplo una violación de índice único o un conflicto de concurrencia
                logger.LogError(ex, "Error al guardar en la base de datos");
                await Responder(contexto, HttpStatusCode.Conflict, "Conflicto",
                    "No se pudo guardar el cambio. Intentá de nuevo.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error no controlado");
                await Responder(contexto, HttpStatusCode.InternalServerError, "Error inesperado",
                    "Ocurrió un error inesperado");
            }
        }

        private static Task Responder(HttpContext contexto, HttpStatusCode estado, string titulo, string mensaje)
        {
            contexto.Response.StatusCode = (int)estado;
            return contexto.Response.WriteAsJsonAsync(
                new { status = (int)estado, title = titulo, mensaje },
                contentType: "application/problem+json");
        }
    }
}
