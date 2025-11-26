namespace Reservas_Laboratorio.Dtos
{
    public class ReservaUpdateDto
    {
        public int Id { get; set; }
        public int LabId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}
