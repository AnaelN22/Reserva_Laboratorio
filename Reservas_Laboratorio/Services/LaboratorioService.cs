using AutoMapper;
using Reservas_Laboratorio.Dtos;
using Reservas_Laboratorio.Repositories;

namespace Reservas_Laboratorio.Services
{
    public class LaboratorioService : ILaboratorioService
    {
        private readonly ILaboratorioRepository _repository;
        private readonly IMapper _mapper;

        public LaboratorioService(ILaboratorioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LaboratorioDto>> GetAllAsync()
        {
            var laboratorios = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<LaboratorioDto>>(laboratorios);
        }
    }
}
