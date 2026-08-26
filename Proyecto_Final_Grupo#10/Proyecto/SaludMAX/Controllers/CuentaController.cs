using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Programacion_Avanzada_Web_G10_SaludMax.Data;
using Programacion_Avanzada_Web_G10_SaludMax.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Programacion_Avanzada_Web_G10_SaludMax.Controllers;

public class CuentaController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<Usuario> _hasher;

    public CuentaController(ApplicationDbContext db, IPasswordHasher<Usuario> hasher) => (_db, _hasher) = (db, hasher);

    [HttpGet]
    public IActionResult IniciarSesion(string? returnUrl = null) => User.Identity?.IsAuthenticated == true
        ? RedirectToAction(nameof(Perfil)) : View(new IniciarSesionViewModel { });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> IniciarSesion(IniciarSesionViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var correo = (model.Correo ?? "").Trim().ToLowerInvariant();
        var usuario = await _db.Usuarios.Include(u => u.Rol).SingleOrDefaultAsync(u => u.Correo == correo);
        var resultado = usuario is null ? PasswordVerificationResult.Failed
            : _hasher.VerifyHashedPassword(usuario, usuario.Contrasena, model.Contrasena);

        if (usuario is not null && resultado == PasswordVerificationResult.Failed && usuario.Contrasena == model.Contrasena)
            resultado = PasswordVerificationResult.SuccessRehashNeeded;
        if (usuario is null || resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(model);
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.Contrasena = _hasher.HashPassword(usuario, model.Contrasena);
            await _db.SaveChangesAsync();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Paciente")
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties { IsPersistent = model.Recordarme });
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action(nameof(Perfil))!);
    }

    [HttpGet]
    public IActionResult Registro() => View(new RegistroViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        model.Correo = (model.Correo ?? "").Trim().ToLowerInvariant();
        if (await _db.Usuarios.AnyAsync(u => u.Correo == model.Correo))
            ModelState.AddModelError(nameof(model.Correo), "Este correo ya está registrado.");
        if (!ModelState.IsValid) return View(model);

        var rol = await _db.Roles.SingleAsync(r => r.Nombre == "Paciente");
        var usuario = new Usuario { Nombre = (model.Nombre ?? "").Trim(), Correo = model.Correo, RolId = rol.Id, Contrasena = "" };
        usuario.Contrasena = _hasher.HashPassword(usuario, model.Contrasena);
        _db.Usuarios.Add(usuario);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException) { ModelState.AddModelError(nameof(model.Correo), "Este correo ya está registrado."); return View(model); }
        TempData["Exito"] = "Cuenta creada correctamente. Ya puedes iniciar sesión.";
        return RedirectToAction(nameof(IniciarSesion));
    }

[Authorize]
public async Task<IActionResult> Perfil()
{
    var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var usuario = await _db.Usuarios
        .Include(u => u.Rol)
        .SingleAsync(u => u.Id == usuarioId);

    var citas = await _db.Citas
        .Include(c => c.ServicioMedico)
        .Include(c => c.Horario)
        .Where(c => c.UsuarioId == usuarioId)
        .OrderByDescending(c => c.Fecha)
        .ToListAsync();

    return View(new PerfilViewModel
    {
        Usuario = usuario,
        Citas = citas
    });
}

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CerrarSesion()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccesoDenegado() => View();




}
