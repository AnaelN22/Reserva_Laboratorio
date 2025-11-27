namespace Reservas_Laboratorio.Dtos
{
    public class ReservaResponseDto
    {
        public int Id { get; set; }
        public int LabId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string UsuarioName { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
    }
}
