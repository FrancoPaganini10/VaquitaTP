using System.Net;
using System.Net.Mail;

namespace Vaquita.Servicios
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<EmailService> logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task Enviar(string destino, string asunto, string cuerpoHtml)
        {
            var host = configuration["Smtp:Host"];

            // Sin SMTP configurado (desarrollo): el mail se muestra en la consola
            if (string.IsNullOrWhiteSpace(host))
            {
                logger.LogInformation("[EMAIL SIMULADO] Para: {Destino} | Asunto: {Asunto} | Cuerpo: {Cuerpo}",
                    destino, asunto, cuerpoHtml);
                return;
            }

            var puerto = configuration.GetValue("Smtp:Port", 587);
            var desde = configuration["Smtp:Desde"]!;

            using var cliente = new SmtpClient(host, puerto)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    configuration["Smtp:Usuario"], configuration["Smtp:Password"])
            };

            using var mensaje = new MailMessage(desde, destino, asunto, cuerpoHtml)
            {
                IsBodyHtml = true
            };

            await cliente.SendMailAsync(mensaje);
        }
    }
}
