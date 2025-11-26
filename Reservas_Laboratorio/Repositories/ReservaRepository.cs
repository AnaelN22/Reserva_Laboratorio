using Reservas_Laboratorio.Data;
using Reservas_Laboratorio.Models;
using Microsoft.EntityFrameworkCore;

namespace Reservas_Laboratorio.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;
        public ReservaRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Reserva>> GetAllAsync()
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Lab)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetByLabAndDateAsync(int labId, DateTime fecha)
        {
            var dateOnly = fecha.Date;
            return await _context.Reservas
                .Where(r => r.LabId == labId && r.Fecha == dateOnly)
                .ToListAsync();
        }

        public async Task<Reserva?> GetByIdAsync(int id) =>
            await _context.Reservas.Include(r => r.Usuario).Include(r => r.Lab).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Reserva> AddAsync(Reserva reserva)
        {
            var e = await _context.Reservas.AddAsync(reserva);
            return e.Entity;
        }

        public async Task UpdateAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            await Task.CompletedTask;
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        // Conflicto: chequear solapamiento (algoritmo de intervalos)
        public async Task<bool> HasConflictAsync(int labId, DateTime fecha, TimeSpan hInicio, TimeSpan hFin, int? excludingReservaId = null)
        {
            var dateOnly = fecha.Date;
            var q = _context.Reservas.Where(r => r.LabId == labId && r.Fecha == dateOnly);

            if (excludingReservaId.HasValue)
                q = q.Where(r => r.Id != excludingReservaId.Value);

            return await q.AnyAsync(r =>
               (r.HoraInicio < hFin) &&
               (hInicio < r.HoraFin)
            );

        }
    }
}
