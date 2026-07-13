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

    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de la cita")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El horario es obligatorio")]
        [Display(Name = "Horario")]
        public int HorarioId { get; set; }
        public Horario Horario { get; set; }

        [Display(Name = "Estado")]
        public EstadoCita Estado { get; set; } = EstadoCita.Solicitada;

        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Display(Name = "Servicio Médico")]
        public int ServicioMedicoId { get; set; }
        public ServicioMedico ServicioMedico { get; set; }
    }
}
