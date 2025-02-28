using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly DocumentosService _documentacionService;
        private readonly ILogger _logger;
        public DocumentosController(DocumentosService documentosService, ILogger<DocumentosService> logger)
        {
            _documentacionService = documentosService;
            _logger = logger;

        }

        [HttpPost("Insertar-Documentacion-Aprobar")]
        public async Task<IActionResult> InsertarDocumentacionYAprobar([FromForm] SolicitudFianzaDocumentoRequest request)
        {
            if (!ModelState.IsValid)
            {
                // Obtener los detalles de los errores del modelo
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                       .Select(e => e.ErrorMessage)
                                                       .ToList();

                // Registrar los errores en la consola o en el log
                foreach (var error in errorMessages)
                {
                    Console.WriteLine(error);  // O usa tu mecanismo de logging
                }

                // Mostrar los errores de validación en TempData
                TempData["ErrorMessage"] = "Datos de documentación no válidos. " +
                                            "Por favor revise los siguientes errores: " + string.Join(", ", errorMessages);

                // Redirigir al usuario con el mensaje de error
                return RedirectToAction("AprobacionSolicitud", "Revision");
            }


            try
            {
                // Verificar si los archivos llegaron correctamente
                if (request.DocumentoSolicitud == null || request.DocumentoConvenio == null || request.DocumentoPagare == null)
                {
                    TempData["ErrorMessage"] = "Faltan archivos requeridos.";
                    return RedirectToAction("AprobacionSolicitud", "Revision");
                }

                // Agregar un log para verificar los valores de los archivos
                Console.WriteLine($"DocumentoSolicitud: {request.DocumentoSolicitud.Length} bytes");
                Console.WriteLine($"DocumentoConvenio: {request.DocumentoConvenio.Length} bytes");
                Console.WriteLine($"DocumentoPagare: {request.DocumentoPagare.Length} bytes");

                // Llamar al servicio para procesar los archivos
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


    }
}
