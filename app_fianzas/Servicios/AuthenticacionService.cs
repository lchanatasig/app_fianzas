using app_fianzas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Threading.Tasks;

namespace app_fianzas.Servicios
{
    public class AuthenticacionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthenticacionService> _logger;
        private readonly AppFianzaUnidosContext _dbContext;

        public AuthenticacionService(IHttpContextAccessor httpContextAccessor, ILogger<AuthenticacionService> logger, AppFianzaUnidosContext dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        /// <summary>
        /// Validacion de credenciales
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="clave"></param>
        /// <returns></returns>
        public async Task<string> ValidarCredencialesAsync(string nickname, string clave)
        {
            string mensaje = string.Empty;
            Usuario usuario = null;

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_validar_login", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    command.Parameters.Add(new SqlParameter("@p_nickname", SqlDbType.NVarChar, 255) { Value = nickname });
                    command.Parameters.Add(new SqlParameter("@p_clave", SqlDbType.NVarChar, 255) { Value = clave });

                    // Parámetro de salida
                    var outputParam = new SqlParameter("@p_mensaje", SqlDbType.NVarChar, 255)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    // Ejecutar el SP y leer los resultados
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Asignar los valores a variables locales
                            int usuarioId = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                            int perfilId = reader.GetInt32(reader.GetOrdinal("id_perfil"));
                            string nombres = reader.GetString(reader.GetOrdinal("nombre_usuario"));
                            string nicknameValue = reader.GetString(reader.GetOrdinal("email"));
                            string perfilNombre = reader.GetString(reader.GetOrdinal("nombre_perfil"));
                            string perfilDescripcion = reader.GetString(reader.GetOrdinal("perfil_descripcion"));

                            // Crear el objeto Usuario
                            usuario = new Usuario
                            {
                                IdUsuario = usuarioId,
                                IdPerfil = perfilId,
                                NombreUsuario = nombres,
                                Email = nicknameValue,
                                IdPerfilNavigation = new Perfil
                                {
                                    NombrePerfil = perfilNombre,
                                    Descripcion = perfilDescripcion
                                }
                            };
                        }
                    }

                    // Obtener el mensaje del SP
                    mensaje = outputParam.Value.ToString();
                }
            }

            // Si se obtuvo un usuario, almacenar sus propiedades en la sesión
            if (usuario != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                httpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
                httpContext.Session.SetInt32("PerfilId", usuario.IdPerfil);
                httpContext.Session.SetString("UsuarioNombre", usuario.NombreUsuario);
                httpContext.Session.SetString("UsuarioPerfil", usuario.IdPerfilNavigation.NombrePerfil);
                httpContext.Session.SetString("UsuarioPerfilDescripcion", usuario.IdPerfilNavigation.Descripcion);
            }

            return mensaje;
        }
    }
}
