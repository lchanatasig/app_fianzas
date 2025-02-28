using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class FianzasController : Controller
    {
        private readonly FianzasService _solicitudFianzaService;
        private readonly ListaService _listaService;
        private readonly EmpresaService _empresaService;

        public FianzasController(FianzasService solicitudFianzaService, ListaService listaService, EmpresaService empresaService)
        {
            _solicitudFianzaService = solicitudFianzaService;
            _listaService = listaService;
            _empresaService = empresaService;
        }

        [HttpGet("Registro-Fianza")]
        public async Task<IActionResult> RegistrarSolicitudFianza()
        {
            ViewBag.TiposFianza = await _listaService.ListarTipoFianzasAsync();
            var (empresas, mensaje) = await _empresaService.ListarEmpresasAsync();
            ViewBag.Empresas = empresas;
            ViewBag.Mensaje = mensaje;  // Opcional, si deseas mostrar el mensaje en la vista

            return View();
        }

        [HttpPost("Registrar-Solicitud-Fianza")]
        public async Task<IActionResult> RegistrarSF(SolicitudFianzaRequest solicitud)
        {
            if (solicitud == null)
            {
                TempData["ErrorMessage"] = "Los datos ingresados no son válidos.";
                return RedirectToAction("RegistrarSolicitudFianza");
            }

            try
            {
                var (idSolicitud, mensaje) = await _solicitudFianzaService.InsertarSolicitudFianzaAsync(solicitud);

                if (idSolicitud.HasValue)
                {
                    TempData["SuccessMessage"] = "Solicitud de fianza registrada exitosamente.";
                    return RedirectToAction("ListarSolicitudesFianza");
                }
                else
                {
                    TempData["ErrorMessage"] = mensaje ?? "No se pudo registrar la solicitud de fianza.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
            }

            return RedirectToAction("RegistrarSolicitudFianza");
        }

        [HttpGet("Solicitudes-Fianza")]
        public async Task<IActionResult> ListarSolicitudesFianza()
        {
            try
            {
                var (solicitudes, mensaje) = await _solicitudFianzaService.ListarSolicitudesFianzaAsync();

                if (!string.IsNullOrEmpty(mensaje))
                {
                    TempData["ErrorMessage"] = mensaje;
                }

                return View(solicitudes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return View(new List<SolicitudFianza>());
            }
        }
    }
}
