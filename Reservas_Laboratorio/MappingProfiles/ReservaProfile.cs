using AutoMapper;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.MappingProfiles
{
    public class ReservaProfile : Profile
    {
        public ReservaProfile()
        {
            // Reserva → ReservaResponseDto
            CreateMap<Reserva, ReservaResponseDto>()
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Lab.LabName))
                .ForMember(dest => dest.UsuarioName, opt => opt.MapFrom(src => src.Usuario.UserName))
                .ForMember(dest => dest.HoraInicio, opt => opt.MapFrom(src => src.HoraInicio))
                .ForMember(dest => dest.HoraFin, opt => opt.MapFrom(src => src.HoraFin));

            // ReservaCreateDto → Reserva
            CreateMap<ReservaCreateDto, Reserva>()
                .ForMember(dest => dest.HoraInicio, opt => opt.MapFrom(src => src.HoraInicio))
                .ForMember(dest => dest.HoraFin, opt => opt.MapFrom(src => src.HoraFin));

            // ReservaUpdateDto → Reserva
            CreateMap<ReservaUpdateDto, Reserva>()
                .ForMember(dest => dest.HoraInicio, opt => opt.MapFrom(src => src.HoraInicio))
                .ForMember(dest => dest.HoraFin, opt => opt.MapFrom(src => src.HoraFin));
        }
    }
}
