namespace Vaquita.Utilidades
{
    public static class Reloj
    {
        private static readonly TimeZoneInfo Zona = ObtenerZona();

        private static TimeZoneInfo ObtenerZona()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires"); }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"); // Windows
            }
        }

        // La fecha de hoy en Argentina, sin hora
        public static DateTime Hoy() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zona).Date;
    }
}