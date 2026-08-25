using System.ComponentModel.DataAnnotations;

namespace Programacion_Avanzada_Web_G10_SaludMax.Models
{
    public class ServicioMedico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre del servicio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = "";

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
