namespace Programacion_Avanzada_Web_G10_SaludMax.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace SaludMAX.Models
    {
        public class Rol
        {
            public int Id { get; set; }

            [Required]
            [Display(Name = "Nombre del rol")]
            public string Nombre { get; set; } = "";

            public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        }
    }
}
