using Reservas_Laboratorio.Data;
using Reservas_Laboratorio.Models;
using Microsoft.EntityFrameworkCore;

namespace Reservas_Laboratorio.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> AddAsync(Usuario user)
        {
            var entry = await _context.Users.AddAsync(user);
            return entry.Entity;
        }

        public async Task<Usuario?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetUserByUserName(string userName)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public bool ValidatePassWord(Usuario user, string passWord)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(passWord, user.Password);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}
