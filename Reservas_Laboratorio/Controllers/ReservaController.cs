using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Models;
using Reservas_Laboratorio.Repositories;

namespace Reservas_Laboratorio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaRepository _repo;
        private readonly IUserRepository _userRepo;

        public ReservaController(IReservaRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repo.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _repo.GetByIdAsync(id);
            if (reserva == null) return NotFound();

            return Ok(reserva);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ReservaCreateDto dto)
        {
            var userName = User.Identity!.Name;
            var user = await _userRepo.GetUserByUserName(userName!);

            if (user == null)
                return Unauthorized();

            if (dto.HoraFin <= dto.HoraInicio)
                return BadRequest("La hora final debe ser mayor que la inicial.");

            // Validar conflicto
            bool conflict = await _repo.HasConflictAsync(
                dto.LabId, dto.Fecha, dto.HoraInicio, dto.HoraFin
            );

            if (conflict)
                return BadRequest("Ya existe una reserva en ese horario.");

            var reserva = new Reserva
            {
                LabId = dto.LabId,
                UsuarioId = user.Id,
                Fecha = dto.Fecha.Date,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin
            };

            await _repo.AddAsync(reserva);
            await _repo.SaveAsync();

            return Ok(reserva);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ReservaUpdateDto dto)
        {
            var userName = User.Identity!.Name;
            var user = await _userRepo.GetUserByUserName(userName!);

            if (user == null)
                return Unauthorized();

            var reserva = await _repo.GetByIdAsync(id);
            if (reserva == null)
                return NotFound();

            if (reserva.UsuarioId != user.Id && !User.IsInRole("Admin"))
                return Forbid();

            if (dto.HoraFin <= dto.HoraInicio)
                return BadRequest("Hora final debe ser mayor que la inicial.");

            bool conflict = await _repo.HasConflictAsync(
                dto.LabId, dto.Fecha, dto.HoraInicio, dto.HoraFin, id
            );

            if (conflict)
                return BadRequest("Conflicto con otra reserva.");

            reserva.LabId = dto.LabId;
            reserva.Fecha = dto.Fecha.Date;
            reserva.HoraInicio = dto.HoraInicio;
            reserva.HoraFin = dto.HoraFin;

            await _repo.UpdateAsync(reserva);
            await _repo.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userName = User.Identity!.Name;
            var user = await _userRepo.GetUserByUserName(userName!);

            if (user == null)
                return Unauthorized();

            var reserva = await _repo.GetByIdAsync(id);
            if (reserva == null)
                return NotFound();

            if (reserva.UsuarioId != user.Id && !User.IsInRole("Admin"))
                return Forbid();

            await _repo.DeleteAsync(reserva);
            await _repo.SaveAsync();

            return NoContent();
        }
    }
}
