using Reservas_Laboratorio.Dtos;

namespace Reservas_Laboratorio.Services
{
    public interface IReservaService
    {
        Task<(bool Success, string? Error, ReservaResponseDto? Data)> CreateReservaAsync(int usuarioId, ReservaCreateDto dto);
        Task<IEnumerable<ReservaResponseDto>> GetAllAsync();
        Task<ReservaResponseDto?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> UpdateReservaAsync(int usuarioId, ReservaUpdateDto dto);
        Task<bool> DeleteReservaAsync(int usuarioId, int reservaId);
    }
}
