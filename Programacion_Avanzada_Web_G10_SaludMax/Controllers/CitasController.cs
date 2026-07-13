using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Data;
using Programacion_Avanzada_Web_G10_SaludMax.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var citas = _context.Citas
                .Include(c => c.Horario)
                .Include(c => c.Usuario)
                .Include(c => c.ServicioMedico);

            return View(await citas.ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Horario)
                .Include(c => c.Usuario)
                .Include(c => c.ServicioMedico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            CargarListas();

            return View(new Cita
            {
                Fecha = DateTime.Today,
                Estado = EstadoCita.Solicitada
            });
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Fecha,HorarioId,Estado,UsuarioId,ServicioMedicoId")]
            Cita cita)
        {
            await ValidarFechaYHorario(cita);

            if (ModelState.IsValid)
            {
                _context.Add(cita);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CargarListas(cita);

            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            CargarListas(cita);

            return View(cita);
        }

        // POST: Citas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Fecha,HorarioId,Estado,UsuarioId,ServicioMedicoId")]
            Cita cita)
        {
            if (id != cita.Id)
            {
                return NotFound();
            }

            await ValidarFechaYHorario(cita);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            CargarListas(cita);

            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Horario)
                .Include(c => c.Usuario)
                .Include(c => c.ServicioMedico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita != null)
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task ValidarFechaYHorario(Cita cita)
        {
            if (cita.HorarioId <= 0)
            {
                return;
            }

            var horario = await _context.Horarios
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == cita.HorarioId);

            if (horario == null)
            {
                ModelState.AddModelError(
                    nameof(Cita.HorarioId),
                    "El horario seleccionado no es válido"
                );

                return;
            }

            if (cita.Fecha == default)
            {
                return;
            }

            var fechaHoraSeleccionada =
                cita.Fecha.Date.Add(horario.Hora);

            if (fechaHoraSeleccionada <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(Cita.HorarioId),
                    "La fecha y el horario seleccionados ya pasaron"
                );
            }
        }

        private void CargarListas(Cita? cita = null)
        {
            var horarios = _context.Horarios
                .OrderBy(h => h.Hora)
                .AsEnumerable()
                .Select(h => new
                {
                    h.Id,
                    Hora = h.Hora.ToString(@"hh\:mm")
                })
                .ToList();

            ViewData["HorarioId"] = new SelectList(
                horarios,
                "Id",
                "Hora",
                cita?.HorarioId
            );

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios.OrderBy(u => u.Nombre),
                "Id",
                "Nombre",
                cita?.UsuarioId
            );

            ViewData["ServicioMedicoId"] = new SelectList(
                _context.ServiciosMedicos.OrderBy(s => s.Nombre),
                "Id",
                "Nombre",
                cita?.ServicioMedicoId
            );
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(c => c.Id == id);
        }
    }
}