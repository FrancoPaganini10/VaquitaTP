using System.Net;
using Vaquita.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Vaquita.Middlewares
{
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
                await Responder(contexto, HttpStatusCode.BadRequest, "Regla de negocio incumplida", ex.Message);
            }
            catch (NoEncontradoException ex)
            {
                logger.LogInformation(ex, "Recurso no encontrado");
                await Responder(contexto, HttpStatusCode.NotFound, "Recurso no encontrado", ex.Message);
            }
            catch (AccesoDenegadoException ex)
            {
                logger.LogInformation(ex, "Acceso denegado");
                await Responder(contexto, HttpStatusCode.Forbidden, "Acceso denegado", ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error no controlado");
                await Responder(contexto, HttpStatusCode.InternalServerError, "Error interno",
                    "Ocurrió un error inesperado");
            }
        }

        private static async Task Responder(HttpContext contexto, HttpStatusCode status,
            string titulo, string detalle)
        {
            contexto.Response.StatusCode = (int)status;

            var problema = new ProblemDetails
            {
                Status = (int)status,
                Title = titulo,
                Detail = detalle
            };

            await contexto.Response.WriteAsJsonAsync(problema, options: null,
                contentType: "application/problem+json");
        }
    }
}
