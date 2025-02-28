using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class RevisionController : Controller
    {
        private readonly RevisionService _revisionService;
        private readonly FianzasService _solicitudFianzaService;

        public RevisionController(RevisionService revisionService, FianzasService solicitudFianzaService)
        {
            _revisionService = revisionService;
            _solicitudFianzaService = solicitudFianzaService;
        }

        [HttpGet("Aprobacion-Solicitud")]
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

        [HttpPost("Aprobar-Solicitud-Tecnico")]
        public async Task<IActionResult> AprobarSolicitudTecnica(AprobacionSolicitudRequest request)
        {
            return await ProcesarSolicitud(request, "Tecnico", true);
        }

        [HttpPost("Rechazar--Solicitud-Tecnico")]
        public async Task<IActionResult> RechazarSolicitudTecnico(AprobacionSolicitudRequest request)
        {
            return await ProcesarSolicitud(request, "Tecnico", false);
        }

        [HttpPost("Aprobar--Solicitud-Legal")]
        public async Task<IActionResult> AprobarSolicitudLegal(AprobacionSolicitudRequest request)
        {
            return await ProcesarSolicitud(request, "Legal", true);
        }

        [HttpPost("Rechazar-Solicitud-Legal")]
        public async Task<IActionResult> RechazarSolicitudLegal(AprobacionSolicitudRequest request)
        {
            return await ProcesarSolicitud(request, "Legal", false);
        }

        private async Task<IActionResult> ProcesarSolicitud(AprobacionSolicitudRequest request, string tipo, bool aprobacion)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error en la validación de los datos.";
                return RedirectToAction("AprobacionSolicitud");
            }

            try
            {
                if (tipo == "Tecnico")
                {
                    request.AprobacionTecnica = aprobacion;
                    request.AprobacionLegal = null; // No afecta lo legal
                }
                else if (tipo == "Legal")
                {
                    request.AprobacionLegal = aprobacion;
                    request.AprobacionTecnica = null; // No afecta lo técnico
                }

                var mensaje = await _revisionService.AprobarSolicitudFianzaAsync(request);

                if (mensaje.Contains("correctamente"))
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
