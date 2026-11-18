using Microsoft.EntityFrameworkCore;
using Vaquita.Entidades;
using Vaquita.Excepciones;

namespace Vaquita.Utilidades
{
    public static class ValidacionesGrupo
    {
        // Devuelve el grupo si existe y el usuario es integrante.
        // Con soloAbierto = true exige además que el grupo siga activo.
        public static async Task<Grupo> ObtenerGrupoComoIntegrante(
            this ApplicationDbContext context, int grupoId, int usuarioId, bool soloAbierto = false)
        {
            var grupo = await context.Grupos.FirstOrDefaultAsync(g => g.Id == grupoId);

            var esIntegrante = grupo is not null && await context.Integrantes
                .AnyAsync(i => i.GrupoId == grupoId && i.UsuarioId == usuarioId);

            // Mismo error exista o no el grupo: así no se puede averiguar qué grupos existen
            if (grupo is null || !esIntegrante)
            {
                throw new NoEncontradoException("El grupo no existe o no sos integrante.");
            }

            if (soloAbierto && !grupo.Activo)
            {
                throw new ReglaDeNegocioException("El grupo está cerrado.");
            }

            return grupo;
        }
    }
}
