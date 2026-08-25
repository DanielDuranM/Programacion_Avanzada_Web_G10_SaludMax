using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Data;
using Programacion_Avanzada_Web_G10_SaludMax.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Controllers
{
    public class ServiciosMedicosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiciosMedicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiciosMedicos
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiciosMedicos.ToListAsync());
        }

        // GET: ServiciosMedicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicioMedico = await _context.ServiciosMedicos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicioMedico == null)
            {
                return NotFound();
            }

            return View(servicioMedico);
        }

        // GET: ServiciosMedicos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiciosMedicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] ServicioMedico servicioMedico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicioMedico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicioMedico);
        }

        // GET: ServiciosMedicos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicioMedico = await _context.ServiciosMedicos.FindAsync(id);
            if (servicioMedico == null)
            {
                return NotFound();
            }
            return View(servicioMedico);
        }

        // POST: ServiciosMedicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] ServicioMedico servicioMedico)
        {
            if (id != servicioMedico.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicioMedico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioMedicoExists(servicioMedico.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(servicioMedico);
        }

        // GET: ServiciosMedicos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicioMedico = await _context.ServiciosMedicos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicioMedico == null)
            {
                return NotFound();
            }

            return View(servicioMedico);
        }

        // POST: ServiciosMedicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicioMedico = await _context.ServiciosMedicos.FindAsync(id);
            if (servicioMedico != null)
            {
                _context.ServiciosMedicos.Remove(servicioMedico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicioMedicoExists(int id)
        {
            return _context.ServiciosMedicos.Any(e => e.Id == id);
        }
    }
}
