using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Usuario user);
        string GenerateRefreshToken();
        string GeneratePasswordResetToken();
    }
}
