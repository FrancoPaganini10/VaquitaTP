using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Vaquita.Excepciones;

namespace Vaquita.Utilidades
{
    public static class ClaimsExtensions
    {
        // El token JWT guarda el Id del usuario en el claim "sub"
        public static int ObtenerUsuarioId(this ClaimsPrincipal user)
        {
            var valor = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(valor, out var id))
            {
                throw new AccesoDenegadoException("No se pudo identificar al usuario.");
            }

            return id;
        }
    }
}
