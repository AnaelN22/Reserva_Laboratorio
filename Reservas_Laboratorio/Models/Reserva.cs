using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Reservas_Laboratorio.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }

        public int LabId { get; set; }
        public virtual Laboratorio? Lab { get; set; }

        // SOLO FECHA (sin hora)
        public DateTime Fecha { get; set; }

        // HORAS
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }

}
