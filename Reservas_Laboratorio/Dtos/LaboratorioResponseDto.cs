namespace Reservas_Laboratorio.Dtos
{
    public class LaboratorioDto
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string Descripcion { get; set; }
        public IEnumerable<ReservaResponseDto> Reservas { get; set; }
    }
}
