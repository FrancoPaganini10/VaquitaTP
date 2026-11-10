namespace Vaquita.Servicios
{
    public interface IEmailService
    {
        Task Enviar(string destino, string asunto, string cuerpoHtml);
    }
}
