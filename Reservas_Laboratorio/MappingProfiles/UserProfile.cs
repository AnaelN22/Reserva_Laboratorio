using AutoMapper;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Usuario, LoginResponseDto>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore());

            CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

        }
    }
}