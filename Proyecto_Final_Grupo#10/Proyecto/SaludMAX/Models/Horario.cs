using System.ComponentModel.DataAnnotations;

namespace Programacion_Avanzada_Web_G10_SaludMax.Models
{
    public class Horario
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Hora")]
        public TimeSpan Hora { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
