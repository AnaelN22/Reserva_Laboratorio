namespace Reservas_Laboratorio.Dtos
{
    public class ReservaCreateDto
    {
        public int LabId { get; set; }
        public DateTime Fecha { get; set; }       // solo fecha (ej: 2025-12-01)
        public DateTime HoraInicio { get; set; } 
        public DateTime HoraFin { get; set; }
    }

}
