using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Models;
using Programacion_Avanzada_Web_G10_SaludMax.Models.SaludMAX.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher<Usuario> hasher, IConfiguration config)
    {
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(new Rol { Nombre = "Administrador" }, new Rol { Nombre = "Paciente" });
            await db.SaveChangesAsync();
        }

        if (!await db.Horarios.AnyAsync())
        {
            db.Horarios.AddRange(Enumerable.Range(8, 9).Select(h => new Horario { Hora = TimeSpan.FromHours(h) }));
            await db.SaveChangesAsync();
        }

        if (!await db.ServiciosMedicos.AnyAsync())
        {
            db.ServiciosMedicos.AddRange(
                new ServicioMedico { Nombre = "Medicina general", Descripcion = "Valoración integral y atención primaria para pacientes de todas las edades." },
                new ServicioMedico { Nombre = "Pediatría", Descripcion = "Atención preventiva y seguimiento de la salud de niños y adolescentes." },
                new ServicioMedico { Nombre = "Nutrición", Descripcion = "Planes de alimentación y acompañamiento nutricional personalizado." });
            await db.SaveChangesAsync();
        }

        var claveAdmin = config["SeedAdmin:Password"];
        var correoAdmin = config["SeedAdmin:Email"] ?? "admin@saludmax.local";
        if (!string.IsNullOrWhiteSpace(claveAdmin) && !await db.Usuarios.AnyAsync(u => u.Correo == correoAdmin))
        {
            var rol = await db.Roles.SingleAsync(r => r.Nombre == "Administrador");
            var admin = new Usuario { Nombre = "Administrador SaludMAX", Correo = correoAdmin, RolId = rol.Id, Contrasena = "" };
            admin.Contrasena = hasher.HashPassword(admin, claveAdmin);
            db.Usuarios.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}
