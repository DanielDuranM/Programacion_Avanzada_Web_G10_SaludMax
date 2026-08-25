using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Programacion_Avanzada_Web_G10_SaludMax.Models;

namespace Programacion_Avanzada_Web_G10_SaludMax.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AcercaDe()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contacto()
        {
            return View(new ContactoViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Contacto(ContactoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            TempData["Exito"] = "Recibimos tu mensaje. Pronto nos pondremos en contacto contigo.";
            return RedirectToAction(nameof(Contacto));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
