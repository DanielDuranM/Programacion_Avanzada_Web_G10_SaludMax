using System.ComponentModel.DataAnnotations;

namespace Programacion_Avanzada_Web_G10_SaludMax.Models
{
    public enum EstadoCita
    {
        Solicitada,
        Confirmada,
        Finalizada,
        Cancelada
    }

    public class Cita : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de la cita")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Horario")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un horario")]
        public int HorarioId { get; set; }

        public Horario? Horario { get; set; }

        [Display(Name = "Estado")]
        [EnumDataType(typeof(EstadoCita))]
        public EstadoCita Estado { get; set; } = EstadoCita.Solicitada;

        [Display(Name = "Usuario")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un usuario")]
        public int UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }

        [Display(Name = "Servicio médico")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio médico")]
        public int ServicioMedicoId { get; set; }

        public ServicioMedico? ServicioMedico { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (Fecha.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha de la cita no puede estar en el pasado",
                    new[] { nameof(Fecha) }
                );
            }
        }
    }
}