using AutoMapper;
using Vaquita.DTOs;
using Vaquita.Entidades;

namespace Vaquita.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UsuarioCreacionDTO, Usuario>();
            CreateMap<GrupoCreacionDTO, Grupo>();

            CreateMap<GastoCreacionDTO, Gasto>()
                .ForMember(g => g.PagadorId, dto => dto.Ignore());
        }
    }
}
