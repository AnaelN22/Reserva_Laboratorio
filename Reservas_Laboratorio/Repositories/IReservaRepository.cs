using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.Repositories
{
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> GetAllAsync();
        Task<IEnumerable<Reserva>> GetByLabAndDateAsync(int labId, DateTime fecha);
        Task<Reserva?> GetByIdAsync(int id);
        Task<Reserva> AddAsync(Reserva reserva);
        Task UpdateAsync(Reserva reserva);
        Task DeleteAsync(Reserva reserva);
        Task<bool> HasConflictAsync(int labId, DateTime fecha, DateTime hInicio, DateTime hFin, int? excludingReservaId = null);
        Task SaveAsync();
    }
}
