using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas_Laboratorio.Services;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Models;
using Reservas_Laboratorio.Repositories;

namespace Reservas_Laboratorio.Controllers
{
    public class LaboratorioController : Controller
    {
        private readonly ILaboratorioService _service;

        public LaboratorioController(ILaboratorioService service)
        {
            _service = service;
        }

        // GET: api/laboratorio
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LaboratorioDto>>> GetAll()
        {
            var laboratorios = await _service.GetAllAsync();
            return Ok(laboratorios);
        }

    }
}
