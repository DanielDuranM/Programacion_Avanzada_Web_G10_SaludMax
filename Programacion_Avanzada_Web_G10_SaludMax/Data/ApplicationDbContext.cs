using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Models;
using Programacion_Avanzada_Web_G10_SaludMax.Models.SaludMAX.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ServicioMedico> ServiciosMedicos { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Horario> Horarios { get; set; }
    }
}
