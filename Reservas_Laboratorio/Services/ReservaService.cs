using AutoMapper;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Models;
using Reservas_Laboratorio.Repositories;

namespace Reservas_Laboratorio.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public ReservaService(IReservaRepository repo, IUserRepository userRepo, IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<(bool Success, string? Error, ReservaResponseDto? Data)> CreateReservaAsync(int usuarioId, ReservaCreateDto dto)
        {
            // Validaciones
            if (dto.HoraFin <= dto.HoraInicio)
                return (false, "HoraFin debe ser mayor que HoraInicio", null);

            if (dto.Fecha.Date < DateTime.UtcNow.Date)
                return (false, "No puedes reservar en fechas pasadas", null);

            // Validación de conflicto
            var conflict = await _repo.HasConflictAsync(dto.LabId, dto.Fecha.Date, dto.HoraInicio, dto.HoraFin);
            if (conflict)
                return (false, "Ya existe una reserva para ese laboratorio y horario", null);

            // Usamos AutoMapper para convertir DTO → Entidad
            var reserva = _mapper.Map<Reserva>(dto);
            reserva.UsuarioId = usuarioId;

            await _repo.AddAsync(reserva);
            await _repo.SaveAsync();

            // Mapear la entidad guardada a ResponseDto
            var saved = await _repo.GetByIdAsync(reserva.Id);
            var response = _mapper.Map<ReservaResponseDto>(saved);

            return (true, null, response);
        }

        public async Task<IEnumerable<ReservaResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ReservaResponseDto>>(list);
        }

        public async Task<ReservaResponseDto?> GetByIdAsync(int id)
        {
            var reserva = await _repo.GetByIdAsync(id);
            if (reserva == null) return null;

            return _mapper.Map<ReservaResponseDto>(reserva);
        }

        public async Task<(bool Success, string? Error)> UpdateReservaAsync(int usuarioId, ReservaUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null)
                return (false, "Reserva no encontrada");

            if (existing.UsuarioId != usuarioId)
                return (false, "No tienes permiso para editar esta reserva");

            if (dto.HoraFin <= dto.HoraInicio)
                return (false, "HoraFin debe ser mayor que HoraInicio");

            if (dto.Fecha.Date < DateTime.UtcNow.Date)
                return (false, "No puedes reservar en fechas pasadas");

            var conflict = await _repo.HasConflictAsync(dto.LabId, dto.Fecha.Date, dto.HoraInicio, dto.HoraFin, dto.Id);
            if (conflict)
                return (false, "Horario en conflicto con otra reserva");

            // Mapear los valores del DTO a la entidad existente
            _mapper.Map(dto, existing);

            await _repo.UpdateAsync(existing);
            await _repo.SaveAsync();

            return (true, null);
        }

        public async Task<bool> DeleteReservaAsync(int usuarioId, int reservaId)
        {
            var existing = await _repo.GetByIdAsync(reservaId);
            if (existing == null) return false;

            await _repo.DeleteAsync(existing);
            await _repo.SaveAsync();

            return true;
        }
    }
}
