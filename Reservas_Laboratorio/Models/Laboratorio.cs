namespace Reservas_Laboratorio.Models
{
    public class Laboratorio
    {
        public int Id { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

}
