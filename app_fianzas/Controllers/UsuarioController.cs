using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly ListaService _listaService;

        public UsuarioController(UsuarioService usuarioService,ListaService listaService)
        {
            _usuarioService = usuarioService;
            _listaService = listaService;
        }

        [HttpGet("Usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            try
            {
                var (usuarios, mensaje) = await _usuarioService.ListarUsuariosAsync();

                if (!string.IsNullOrEmpty(mensaje))
                {
                    TempData["ErrorMessage"] = mensaje;
                }

                return View(usuarios);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return View(new List<Usuario>());
            }
        }



        [HttpGet("Registro")]
        public async Task <IActionResult> NuevoUsuario()
        {
            var perfiles = await _listaService.ListarPerfilesAsync();
            ViewBag.Perfiles = perfiles;
            return View();
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromForm] Usuario usuarioModel)
        {
            if (usuarioModel == null)
            {
                TempData["ErrorMessage"] = "Datos inválidos. Por favor, complete el formulario.";
                return RedirectToAction("Registro");
            }

            try
            {
                var (idUsuario, mensaje) = await _usuarioService.InsertarUsuarioAsync(
                    usuarioModel.IdPerfil,
                    usuarioModel.NombreUsuario,
                    usuarioModel.Contrasena,
                    usuarioModel.Email
                );

                if (idUsuario.HasValue)
                {
                    TempData["SuccessMessage"] = "Usuario registrado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = mensaje ?? "No se pudo registrar el usuario.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
            }

            return RedirectToAction("Registro");
        }


        /// <summary>
        /// Metodo que trae la informacion del usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet("Usuario/Detalle/{idUsuario}")]
        public async Task<IActionResult> DetalleUsuario(int idUsuario)
        {
            try
            {
                var (usuario, mensaje) = await _usuarioService.ObtenerUsuarioPorIdAsync(idUsuario);

                if (usuario == null)
                {
                    TempData["ErrorMessage"] = mensaje ?? "No se encontró el usuario.";
                    return RedirectToAction("ListarUsuarios");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction("ListarUsuarios");
            }
        }

        /// <summary>
        /// CONTROLADOR VISTA
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        [HttpGet("Actualizar-Usuario/{usuarioId}")]
        public async Task<IActionResult> ActualizarUsuario(int usuarioId)
        {
            try
            {
                // Obtener la lista de perfiles
                var perfiles = await _listaService.ListarPerfilesAsync();
                ViewBag.Perfiles = perfiles;

                // Obtener los datos del usuario a actualizar
                var (usuario, mensaje) = await _usuarioService.ObtenerUsuarioPorIdAsync(usuarioId);

                if (usuario == null)
                {
                    TempData["ErrorMessage"] = mensaje ?? "Usuario no encontrado.";
                    return RedirectToAction("ListarUsuarios");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction("ListarUsuarios");
            }
        }

        /// <summary>
        /// Controlador METODO QUE LLAMA AL SERVICIO
        /// </summary>
        /// <param name="usuarioActualizado"></param>
        /// <returns></returns>
        [HttpPost("Actualizar-Usuario")]
        public async Task<IActionResult> ActualizarU(Usuario usuarioActualizado)
        {
            if (usuarioActualizado == null)
            {
                TempData["ErrorMessage"] = "Los datos del usuario no son válidos.";
                return RedirectToAction("ActualizarUsuario");
            }

            try
            {
                var mensaje = await _usuarioService.ActualizarUsuarioAsync(
                    usuarioActualizado.IdUsuario,
                    usuarioActualizado.IdPerfil,
                    usuarioActualizado.NombreUsuario,
                    usuarioActualizado.Contrasena,
                    usuarioActualizado.Email,
                    usuarioActualizado.EstadoUsuario ?? 1
                );

                if (string.IsNullOrEmpty(mensaje) || mensaje.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = mensaje;
                    return RedirectToAction("ActualizarUsuario");

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
                return View();
            }

            return RedirectToAction("ListarUsuarios", new { usuarioId = usuarioActualizado.IdUsuario });
        }


        /// <summary>
        /// COntrolador para el cambio de estado
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="nuevoEstado"></param>
        /// <returns></returns>
        [HttpPost("CambiarEstadoUsuario")]
        public async Task<IActionResult> CambiarEstadoUsuario(int idUsuario, byte nuevoEstado)
        {
            try
            {
                var mensaje = await _usuarioService.CambiarEstadoUsuarioAsync(idUsuario, nuevoEstado);

                if (string.IsNullOrEmpty(mensaje) || mensaje.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["SuccessMessage"] = "Estado del usuario actualizado correctamente.";
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

            return RedirectToAction("ListarUsuarios");
        }
   
    
    }
}
