using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Data;
using Programacion_Avanzada_Web_G10_SaludMax.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Controllers;

[Authorize]
public class CitasController : Controller
{
    private readonly ApplicationDbContext _db;
    public CitasController(ApplicationDbContext db) => _db = db;
    private bool EsAdmin => User.IsInRole("Administrador");
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private IQueryable<Cita> Consulta() => _db.Citas.Include(c => c.Horario).Include(c => c.Usuario).Include(c => c.ServicioMedico);

    public async Task<IActionResult> Index()
    {
        var query = Consulta();
        if (!EsAdmin) query = query.Where(c => c.UsuarioId == UsuarioId);
        return View(await query.OrderByDescending(c => c.Fecha).ThenBy(c => c.Horario!.Hora).ToListAsync());
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Pendientes() => View(await Consulta()
        .Where(c => c.Estado == EstadoCita.Solicitada).OrderBy(c => c.Fecha).ThenBy(c => c.Horario!.Hora).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> HorariosDisponibles(DateTime fecha, int citaId = 0)
    {
        var ocupados = await _db.Citas.Where(c => c.Id != citaId && c.Fecha == fecha.Date && c.Estado != EstadoCita.Cancelada)
            .Select(c => c.HorarioId).ToListAsync();
        var horarios = await _db.Horarios.Where(h => !ocupados.Contains(h.Id)).OrderBy(h => h.Hora).ToListAsync();
        return Json(horarios.Select(h => new { h.Id, Hora = h.Hora.ToString(@"hh\:mm") }));
    }

    public async Task<IActionResult> Details(int? id)
    {
        var cita = await Consulta().FirstOrDefaultAsync(c => c.Id == id && (EsAdmin || c.UsuarioId == UsuarioId));
        return cita is null ? NotFound() : View(cita);
    }

    public IActionResult Create(int? servicioId)
    {
        var cita = new Cita { Fecha = DateTime.Today.AddDays(1), Estado = EstadoCita.Solicitada, ServicioMedicoId = servicioId ?? 0 };
        CargarListas(cita);
        return View(cita);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Fecha,HorarioId,Estado,UsuarioId,ServicioMedicoId")] Cita cita)
    {
        if (!EsAdmin)
        {
            cita.UsuarioId = UsuarioId;
            cita.Estado = EstadoCita.Solicitada;
            ModelState.Remove(nameof(cita.UsuarioId));
            ModelState.Remove(nameof(cita.Estado));
        }
        await ValidarDisponibilidad(cita);
        if (!ModelState.IsValid) { CargarListas(cita); return View(cita); }
        _db.Add(cita);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(cita.HorarioId), "Ese horario acaba de ser reservado. Selecciona otro."); CargarListas(cita); return View(cita); }
        TempData["Exito"] = "La cita fue solicitada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int? id)
    {
        var cita = await _db.Citas.FindAsync(id);
        if (cita is null) return NotFound();
        CargarListas(cita);
        return View(cita);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,HorarioId,Estado,UsuarioId,ServicioMedicoId")] Cita cita)
    {
        if (id != cita.Id) return NotFound();
        await ValidarDisponibilidad(cita);
        if (!ModelState.IsValid) { CargarListas(cita); return View(cita); }
        _db.Update(cita);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(cita.HorarioId), "Ese horario acaba de ser reservado. Selecciona otro."); CargarListas(cita); return View(cita); }
        TempData["Exito"] = "La cita fue actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int? id)
    {
        var cita = await Consulta().FirstOrDefaultAsync(c => c.Id == id);
        return cita is null ? NotFound() : View(cita);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cita = await _db.Citas.FindAsync(id);
        if (cita is not null) { _db.Citas.Remove(cita); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == id && (EsAdmin || c.UsuarioId == UsuarioId));
        if (cita is null) return NotFound();
        if (cita.Estado is EstadoCita.Solicitada or EstadoCita.Confirmada)
        {
            cita.Estado = EstadoCita.Cancelada;
            await _db.SaveChangesAsync();
            TempData["Exito"] = "La cita fue cancelada.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarDisponibilidad(Cita cita)
    {
        var horario = await _db.Horarios.AsNoTracking().FirstOrDefaultAsync(h => h.Id == cita.HorarioId);
        if (horario is null) return;
        if (cita.Fecha.Date.Add(horario.Hora) <= DateTime.Now)
            ModelState.AddModelError(nameof(cita.HorarioId), "La fecha y el horario seleccionados ya pasaron.");
        if (await _db.Citas.AnyAsync(c => c.Id != cita.Id && c.Fecha == cita.Fecha.Date && c.HorarioId == cita.HorarioId && c.Estado != EstadoCita.Cancelada))
            ModelState.AddModelError(nameof(cita.HorarioId), "Ese horario ya está reservado. Selecciona otro.");
    }

    private void CargarListas(Cita cita)
    {
        ViewData["HorarioId"] = new SelectList(_db.Horarios.OrderBy(h => h.Hora).AsEnumerable()
            .Select(h => new { h.Id, Hora = h.Hora.ToString(@"hh\:mm") }), "Id", "Hora", cita.HorarioId);
        ViewData["ServicioMedicoId"] = new SelectList(_db.ServiciosMedicos.OrderBy(s => s.Nombre), "Id", "Nombre", cita.ServicioMedicoId);
        if (EsAdmin) ViewData["UsuarioId"] = new SelectList(_db.Usuarios.OrderBy(u => u.Nombre), "Id", "Nombre", cita.UsuarioId);
    }
}
