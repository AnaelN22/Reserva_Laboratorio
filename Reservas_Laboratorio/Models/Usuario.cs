using System.Data;

namespace Reservas_Laboratorio.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiryTime { get; set; }

        public int RoleId { get; set; }
        public virtual Role? Role { get; set; }
    }

}
