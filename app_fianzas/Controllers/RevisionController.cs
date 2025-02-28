using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class RevisionController : Controller
    {
        private readonly FianzasService _solicitudFianzaService;
        private readonly ListaService _listaService;
        private readonly EmpresaService _empresaService;
        private readonly RevisionService _revisionService;

        public RevisionController(FianzasService solicitudFianzaService, ListaService listaService, EmpresaService empresaService, RevisionService revisionService)
        {
            _solicitudFianzaService = solicitudFianzaService;
            _listaService = listaService;
            _empresaService = empresaService;
            _revisionService = revisionService;
        }

        [HttpGet("Aprobación")]
        public async Task<IActionResult> AprobacionSolicitud()
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

        [HttpPost("Procesar-Solicitud")]
        public async Task<IActionResult> ProcesarSolicitud(AprobacionSolicitudRequest request, bool esAprobacion)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error en la validación de los datos.";
                return RedirectToAction("AprobacionSolicitud");
            }

            try
            {
                // Definir aprobación o rechazo
                request.AprobacionTecnica = esAprobacion;
                request.AprobacionLegal = esAprobacion;

                var mensaje = await _revisionService.AprobarSolicitudFianzaAsync(request);

                if (mensaje.Contains("exitosamente"))
                {
                    TempData["SuccessMessage"] = mensaje;
                }
                else
                {
                    TempData["ErrorMessage"] = mensaje;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
            }

            return RedirectToAction("AprobacionSolicitud");
        }
    }
}
