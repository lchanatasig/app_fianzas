using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace app_fianzas.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly DocumentosService _documentacionService;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public DocumentosController(DocumentosService documentosService, ILogger<DocumentosService> logger, IWebHostEnvironment env)
        {
            _documentacionService = documentosService;
            _logger = logger;
            _env = env;
        }

        [HttpPost("Insertar-Documentacion-Aprobar")]
        public async Task<IActionResult> InsertarDocumentacionYAprobar([FromForm] SolicitudFianzaDocumentoRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                       .Select(e => e.ErrorMessage)
                                                       .ToList();

                foreach (var error in errorMessages)
                {
                    Console.WriteLine(error);
                }

                TempData["ErrorMessage"] = "Datos de documentación no válidos. " +
                                           "Por favor revise los siguientes errores: " + string.Join(", ", errorMessages);
                return RedirectToAction("AprobacionSolicitud", "Revision");
            }

            try
            {
                if (request.DocumentoSolicitud == null || request.DocumentoConvenio == null || request.DocumentoPagare == null)
                {
                    TempData["ErrorMessage"] = "Faltan archivos requeridos.";
                    return RedirectToAction("AprobacionSolicitud", "Revision");
                }

                Console.WriteLine($"DocumentoSolicitud: {request.DocumentoSolicitud.Length} bytes");
                Console.WriteLine($"DocumentoConvenio: {request.DocumentoConvenio.Length} bytes");
                Console.WriteLine($"DocumentoPagare: {request.DocumentoPagare.Length} bytes");

                var mensaje = await _documentacionService.InsertarDocumentacionYAprobarAsync(request);

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

            return RedirectToAction("AprobacionSolicitud", "Revision");
        }

        [HttpGet("DescargarPdf")]
        public IActionResult DescargarPdf(string tipo)
        {
            string fileName;
            switch (tipo)
            {
                case "solicitud":
                    fileName = "solicitud.pdf";
                    break;
                case "convenio":
                    fileName = "convenio.pdf";
                    break;
                case "pagare":
                    fileName = "pagare.pdf";
                    break;
                case "prenda":
                    fileName = "prenda.pdf";
                    break;
                default:
                    return NotFound();
            }

            // _env.WebRootPath apunta a "C:\Users\SAFERISK\source\repos\app_fianzas\app_fianzas\wwwroot"
            var filePath = Path.Combine(_env.WebRootPath, "plantillas", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
