using Reservas_Laboratorio.Dtos;

namespace Reservas_Laboratorio.Services
{
    public interface ILaboratorioService
    {
        Task<IEnumerable<LaboratorioDto>> GetAllAsync();
    }
}
