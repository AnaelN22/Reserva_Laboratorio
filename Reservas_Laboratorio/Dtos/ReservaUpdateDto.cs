namespace Reservas_Laboratorio.Dtos
{
    public class ReservaUpdateDto
    {
        public int Id { get; set; }
        public int LabId { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
    }
}
