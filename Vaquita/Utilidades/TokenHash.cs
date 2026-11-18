using System.Security.Cryptography;
using System.Text;

namespace Vaquita.Utilidades
{
    // Los tokens (login e invitaciones) viajan por mail en texto plano pero en la base
    // se guarda solo su hash SHA-256: si se filtra la base, los links no sirven.
    // Los tokens son aleatorios de 256 bits, por eso alcanza con un hash rápido sin sal.
    public static class TokenHash
    {
        public static string Hashear(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
