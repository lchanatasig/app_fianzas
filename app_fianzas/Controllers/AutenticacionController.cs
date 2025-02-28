using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly AuthenticacionService _authenticacionService;

        public AutenticacionController(AuthenticacionService usuarioService)
        {
            _authenticacionService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
   
        public async Task<IActionResult> Validar(string nickname, string clave)
        {
            // Validación básica de los parámetros
            if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(clave))
            {
                return Json(new { success = false, errorMessage = "Nickname y clave son requeridos." });
            }

            // Llamada al servicio para validar las credenciales
            string mensaje = await _authenticacionService.ValidarCredencialesAsync(nickname, clave);

            // Si se devuelve un mensaje distinto al de éxito, se interpreta como error
            if (!string.IsNullOrEmpty(mensaje) && mensaje != "Credenciales validadas exitosamente.")
            {
                return Json(new { success = false, errorMessage = mensaje });
            }


            // Credenciales correctas: redirige sin mensaje de éxito
            return Json(new { success = true, redirectUrl = Url.Action("Index", "Dashboard") });
        }
    }
}
