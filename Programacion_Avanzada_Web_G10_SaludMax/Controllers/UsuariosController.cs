using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Data;
using Programacion_Avanzada_Web_G10_SaludMax.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Usuario> _hasher;

        public UsuariosController(ApplicationDbContext context, IPasswordHasher<Usuario> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Usuarios.Include(u => u.Rol);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Correo,Contrasena,RolId")] Usuario usuario)
        {
            usuario.Correo = (usuario.Correo ?? "").Trim().ToLowerInvariant();
            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo))
                ModelState.AddModelError(nameof(usuario.Correo), "Este correo ya está registrado.");
            if (ModelState.IsValid)
            {
                usuario.Contrasena = _hasher.HashPassword(usuario, usuario.Contrasena);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "El usuario fue creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.Contrasena = "";
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo,Contrasena,RolId")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            var actual = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
            if (actual == null) return NotFound();
            usuario.Correo = (usuario.Correo ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(usuario.Contrasena)) ModelState.Remove(nameof(usuario.Contrasena));
            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != id))
                ModelState.AddModelError(nameof(usuario.Correo), "Este correo ya está registrado.");
            if (ModelState.IsValid)
            {
                usuario.Contrasena = string.IsNullOrWhiteSpace(usuario.Contrasena)
                    ? actual.Contrasena : _hasher.HashPassword(usuario, usuario.Contrasena);
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Exito"] = "El usuario fue actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            TempData["Exito"] = "El usuario fue eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
