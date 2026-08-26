using Programacion_Avanzada_Web_G10_SaludMax.Models.SaludMAX.Models;
using System.ComponentModel.DataAnnotations;

namespace Programacion_Avanzada_Web_G10_SaludMax.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        [StringLength(160)]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(255)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Contrasena { get; set; } = "";

        [Display(Name = "Rol")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol")]
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
