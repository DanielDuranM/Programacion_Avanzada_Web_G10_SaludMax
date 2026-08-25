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

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<ServicioMedico> ServiciosMedicos => Set<ServicioMedico>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Horario> Horarios => Set<Horario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Correo).IsUnique();
            modelBuilder.Entity<Cita>().HasIndex(c => new { c.Fecha, c.HorarioId }).IsUnique().HasFilter("[Estado] <> 3");
            modelBuilder.Entity<Cita>().HasOne(c => c.Usuario).WithMany(u => u.Citas)
                .HasForeignKey(c => c.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Cita>().HasOne(c => c.ServicioMedico).WithMany(s => s.Citas)
                .HasForeignKey(c => c.ServicioMedicoId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
