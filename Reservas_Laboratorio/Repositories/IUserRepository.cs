using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.Repositories
{
    public interface IUserRepository
    {
        Task<Usuario> AddAsync(Usuario user);
        Task<Usuario?> GetUserByEmail(string email);
        Task<Usuario?> GetUserByUserName(string userName);
        bool ValidatePassWord(Usuario user, string passWord);
        Task SaveAsync();
        Task<Usuario?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
