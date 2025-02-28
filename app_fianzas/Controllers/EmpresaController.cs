using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly EmpresaService _empresaService;

        private readonly ListaService _listaService;

        public EmpresaController(EmpresaService empresaService, ListaService listaService)
        {
            _empresaService = empresaService;
            _listaService = listaService;
        }

        /// <summary>
        /// Listado de las empresas
        /// </summary>
        /// <returns></returns>
        [HttpGet("Empresas")]
        public async Task<IActionResult> ListarEmpresas()
        {
            try
            {
                var (empresas, mensaje) = await _empresaService.ListarEmpresasAsync();

                if (!string.IsNullOrEmpty(mensaje))
                {
                    TempData["ErrorMessage"] = mensaje;
                }

                return View(empresas);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return View(new List<Empresa>());
            }
        }

        /// <summary>
        /// VIsta de crear empresas
        /// </summary>
        /// <returns></returns>
        [HttpGet("Registrar-Empresa")]
        public async Task<IActionResult> RegistrarEmpresa()
        {
            var tipoEmpresas = await _listaService.ListarTipoEmpresaAsync();
            ViewBag.TipoEmpresa = tipoEmpresas;
            return View();
        }
        /// <summary>
        /// metodo post
        /// </summary>
        /// <param name="empresaModel"></param>
        /// <returns></returns>
        [HttpPost("Registrar-Empresa")]
        public async Task<IActionResult> RegistrarE(EmpresaRegistroModel empresaModel)
        {
            if (empresaModel == null)
            {
                TempData["ErrorMessage"] = "Los datos ingresados no son válidos.";
                return RedirectToAction("RegistrarEmpresa");
            }

            try
            {
                var (idEmpresa, mensaje) = await _empresaService.InsertarEmpresaAsync(
                    empresaModel.TipoEmpresaId,
                    empresaModel.NombreEmpresa,
                    empresaModel.DireccionEmpresa,
                    empresaModel.CiEmpresa,
                    empresaModel.TelefonoEmpresa,
                    empresaModel.EmailEmpresa,
                    empresaModel.ActivoCorriente,
                    empresaModel.ActivoFijo,
                    empresaModel.Capital,
                    empresaModel.Reserva,
                    empresaModel.Perdida,
                    empresaModel.Ventas,
                    empresaModel.Utilidad,
                    empresaModel.CupoTotal
                );

                if (idEmpresa.HasValue)
                {
                    TempData["SuccessMessage"] = "Empresa registrada exitosamente.";
                    return RedirectToAction("ListarEmpresas");
                }
                else
                {
                    TempData["ErrorMessage"] = mensaje ?? "No se pudo registrar la empresa.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
            }

            return RedirectToAction("RegistrarEmpresa");
        }

        /// <summary>
        /// Vista actualizacion
        /// </summary>
        /// <param name="empresaId"></param>
        /// <returns></returns>
        [HttpGet("Actualizar-Empresa/{empresaId}")]
        public async Task<IActionResult> ActualizarEmpresa(int empresaId)
        {
            try
            {
                // Obtener la lista de tipos de empresa
                var tiposEmpresa = await _listaService.ListarTipoEmpresaAsync();
                ViewBag.TiposEmpresa = tiposEmpresa;

                // Obtener los datos de la empresa a actualizar
                var (empresa, mensaje) = await _empresaService.ObtenerEmpresaPorIdAsync(empresaId);

                if (empresa == null)
                {
                    TempData["ErrorMessage"] = mensaje ?? "Empresa no encontrada.";
                    return RedirectToAction("ListarEmpresas");
                }

                return View(empresa);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction("ListarEmpresas");
            }
        }

        /// <summary>
        /// mmetodo post actualizar
        /// </summary>
        /// <param name="empresaActualizada"></param>
        /// <returns></returns>
        [HttpPost("Actualizar-Empresa")]
        public async Task<IActionResult> ActualizarE(EmpresaRegistroModel empresaActualizada)
        {
            if (empresaActualizada == null)
            {
                TempData["ErrorMessage"] = "Los datos de la empresa no son válidos.";
                return RedirectToAction("ListarEmpresas");
            }

            try
            {
                var mensaje = await _empresaService.ActualizarEmpresaAsync(
                    empresaActualizada.EmpresaId,
                    empresaActualizada.TipoEmpresaId,
                    empresaActualizada.NombreEmpresa,
                    empresaActualizada.DireccionEmpresa,
                    empresaActualizada.CiEmpresa,
                    empresaActualizada.TelefonoEmpresa,
                    empresaActualizada.EmailEmpresa,
                    empresaActualizada.ActivoCorriente,
                    empresaActualizada.ActivoFijo,
                    empresaActualizada.Capital,
                    empresaActualizada.Reserva,
                    empresaActualizada.Perdida,
                    empresaActualizada.Ventas,
                    empresaActualizada.Utilidad,
                    empresaActualizada.CupoTotal
                );

                if (string.IsNullOrEmpty(mensaje) || mensaje.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["SuccessMessage"] = "Empresa actualizada correctamente.";
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

            return RedirectToAction("ListarEmpresas", new { empresaId = empresaActualizada.EmpresaId });
        }


        /// <summary>
        /// Traer el historico 
        ///
        /// </summary>
        /// <param name="empresaId"></param>
        /// <returns></returns>
        [HttpGet("Historial-Empresa/{empresaId}")]
        public async Task<IActionResult> HistorialEmpresa(int empresaId)
        {
            try
            {
                var (historial, mensaje) = await _empresaService.ObtenerHistorialEmpresaAsync(empresaId);

                if (!string.IsNullOrEmpty(mensaje))
                {
                    TempData["ErrorMessage"] = mensaje;
                }

                return View(historial);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return View(new List<HistorialEmpresa>());
            }
        }
    }
}
