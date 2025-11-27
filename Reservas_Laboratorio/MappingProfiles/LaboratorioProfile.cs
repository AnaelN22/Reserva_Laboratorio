using AutoMapper;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.MappingProfiles
{
    public class LaboratorioProfile : Profile
    {
        public LaboratorioProfile() {
            // Laboratorio → LaboratorioDto (para creación/actualización)
            CreateMap<Laboratorio, LaboratorioDto>()
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion));

            // LaboratorioDto → Laboratorio (para recibir datos del cliente)
            CreateMap<LaboratorioDto, Laboratorio>()
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion));

            // Laboratorio → LaboratorioResponseDto (para enviar datos con reservas)
            CreateMap<Laboratorio, LaboratorioResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Reservas, opt => opt.MapFrom(src => src.Reservas));
        }
        
    }
}
