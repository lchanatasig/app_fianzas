using app_fianzas.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace app_fianzas.Servicios
{
    public class UsuarioService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsuarioService> _logger;
        private readonly AppFianzaUnidosContext _dbContext;


        public UsuarioService(IHttpContextAccessor httpContextAccessor, ILogger<UsuarioService> logger, AppFianzaUnidosContext dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Servicio de insertar un nuevo usuario consume el sp
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="contrasena"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<(int? IdUsuario, string Mensaje)> InsertarUsuarioAsync(int idPerfil, string nombreUsuario, string contrasena, string email)
        {
            int? nuevoIdUsuario = null;
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_insert_usuario";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    var idPerfilParam = command.CreateParameter();
                    idPerfilParam.ParameterName = "@id_perfil";
                    idPerfilParam.Value = idPerfil;
                    idPerfilParam.DbType = DbType.Int32;
                    command.Parameters.Add(idPerfilParam);

                    var nombreUsuarioParam = command.CreateParameter();
                    nombreUsuarioParam.ParameterName = "@nombre_usuario";
                    nombreUsuarioParam.Value = nombreUsuario;
                    nombreUsuarioParam.DbType = DbType.String;
                    command.Parameters.Add(nombreUsuarioParam);

                    var contrasenaParam = command.CreateParameter();
                    contrasenaParam.ParameterName = "@contrasena";
                    contrasenaParam.Value = contrasena;
                    contrasenaParam.DbType = DbType.String;
                    command.Parameters.Add(contrasenaParam);

                    var emailParam = command.CreateParameter();
                    emailParam.ParameterName = "@email";
                    emailParam.Value = email;
                    emailParam.DbType = DbType.String;
                    command.Parameters.Add(emailParam);

                    // Parámetro de salida
                    var mensajeParam = command.CreateParameter();
                    mensajeParam.ParameterName = "@p_mensaje";
                    mensajeParam.DbType = DbType.String;
                    mensajeParam.Size = 4000;
                    mensajeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync() && !reader.IsDBNull(reader.GetOrdinal("nuevo_id_usuario")))
                        {
                            nuevoIdUsuario = reader.GetInt32(reader.GetOrdinal("nuevo_id_usuario"));
                        }
                    }

                    mensaje = mensajeParam.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al insertar usuario: {ex.Message}");
                mensaje = "Error inesperado al insertar el usuario.";
            }

            return (nuevoIdUsuario, mensaje);
        }

        /// <summary>
        /// Servicio de Listar todos los usuarios
        /// </summary>
        /// <returns></returns>
        public async Task<(List<Usuario> Usuarios, string Mensaje)> ListarUsuariosAsync()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_listar_usuarios";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro de salida para capturar mensajes o errores
                    var mensajeParam = command.CreateParameter();
                    mensajeParam.ParameterName = "@p_mensaje";
                    mensajeParam.DbType = DbType.String;
                    mensajeParam.Size = 4000;
                    mensajeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var usuario = new Usuario
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                EstadoUsuario = reader.GetByte(reader.GetOrdinal("estado_usuario")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                                IdPerfil = reader.GetInt32(reader.GetOrdinal("id_perfil")),
                                IdPerfilNavigation = new Perfil
                                {
                                    NombrePerfil = reader.GetString(reader.GetOrdinal("nombre_perfil")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("perfil_descripcion"))
                                }
                            };
                            listaUsuarios.Add(usuario);
                        }
                    }

                    mensaje = mensajeParam.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al listar usuarios: {ex.Message}");
                mensaje = "Error inesperado al listar los usuarios.";
            }

            return (listaUsuarios, mensaje);
        }

        /// <summary>
        /// Obtener Usuario por un Id
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public async Task<(Usuario Usuario, string Mensaje)> ObtenerUsuarioPorIdAsync(int idUsuario)
        {
            Usuario usuario = null;
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_get_usuario";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada
                    var idUsuarioParam = command.CreateParameter();
                    idUsuarioParam.ParameterName = "@id_usuario";
                    idUsuarioParam.Value = idUsuario;
                    idUsuarioParam.DbType = DbType.Int32;
                    command.Parameters.Add(idUsuarioParam);

                    // Parámetro de salida
                    var mensajeParam = command.CreateParameter();
                    mensajeParam.ParameterName = "@p_mensaje";
                    mensajeParam.DbType = DbType.String;
                    mensajeParam.Size = 4000;
                    mensajeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                EstadoUsuario = reader.GetByte(reader.GetOrdinal("estado_usuario")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                                IdPerfil = reader.GetInt32(reader.GetOrdinal("id_perfil")),
                                IdPerfilNavigation = new Perfil
                                {
                                    NombrePerfil = reader.GetString(reader.GetOrdinal("nombre_perfil")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("perfil_descripcion"))
                                }
                            };
                        }
                    }

                    mensaje = mensajeParam.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario con ID {idUsuario}: {ex.Message}");
                mensaje = "Error inesperado al obtener el usuario.";
            }

            return (usuario, mensaje);
        }

        /// <summary>
        /// Metodo actualizar un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idPerfil"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="contrasena"></param>
        /// <param name="email"></param>
        /// <param name="estadoUsuario"></param>
        /// <returns></returns>
        public async Task<string> ActualizarUsuarioAsync(int idUsuario, int idPerfil, string nombreUsuario, string? contrasena, string email, byte estadoUsuario)
        {
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_actualizar_usuario";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    var idUsuarioParam = command.CreateParameter();
                    idUsuarioParam.ParameterName = "@id_usuario";
                    idUsuarioParam.Value = idUsuario;
                    idUsuarioParam.DbType = DbType.Int32;
                    command.Parameters.Add(idUsuarioParam);

                    var idPerfilParam = command.CreateParameter();
                    idPerfilParam.ParameterName = "@id_perfil";
                    idPerfilParam.Value = idPerfil;
                    idPerfilParam.DbType = DbType.Int32;
                    command.Parameters.Add(idPerfilParam);

                    var nombreUsuarioParam = command.CreateParameter();
                    nombreUsuarioParam.ParameterName = "@nombre_usuario";
                    nombreUsuarioParam.Value = nombreUsuario;
                    nombreUsuarioParam.DbType = DbType.String;
                    command.Parameters.Add(nombreUsuarioParam);

                    var contrasenaParam = command.CreateParameter();
                    contrasenaParam.ParameterName = "@contrasena";
                    contrasenaParam.Value = (object?)contrasena ?? DBNull.Value;
                    contrasenaParam.DbType = DbType.String;
                    command.Parameters.Add(contrasenaParam);

                    var emailParam = command.CreateParameter();
                    emailParam.ParameterName = "@email";
                    emailParam.Value = email;
                    emailParam.DbType = DbType.String;
                    command.Parameters.Add(emailParam);

                    var estadoUsuarioParam = command.CreateParameter();
                    estadoUsuarioParam.ParameterName = "@estado_usuario";
                    estadoUsuarioParam.Value = estadoUsuario;
                    estadoUsuarioParam.DbType = DbType.Byte;
                    command.Parameters.Add(estadoUsuarioParam);

                    // Parámetro de salida para capturar mensaje
                    var mensajeParam = command.CreateParameter();
                    mensajeParam.ParameterName = "@p_mensaje";
                    mensajeParam.DbType = DbType.String;
                    mensajeParam.Size = 4000;
                    mensajeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    await command.ExecuteNonQueryAsync();

                    // Obtener mensaje del SP
                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al actualizar el usuario.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar usuario con ID {idUsuario}: {ex.Message}");
                mensaje = "Error inesperado al actualizar el usuario.";
            }

            return mensaje;
        }

        /// <summary>
        /// Cambio de estado
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="nuevoEstado"></param>
        /// <returns></returns>
        public async Task<string> CambiarEstadoUsuarioAsync(int idUsuario, byte nuevoEstado)
        {
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_activar_desactivar_usuario";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada: ID del usuario
                    var idUsuarioParam = command.CreateParameter();
                    idUsuarioParam.ParameterName = "@id_usuario";
                    idUsuarioParam.Value = idUsuario;
                    idUsuarioParam.DbType = DbType.Int32;
                    command.Parameters.Add(idUsuarioParam);

                    // Parámetro de entrada: Nuevo estado
                    var nuevoEstadoParam = command.CreateParameter();
                    nuevoEstadoParam.ParameterName = "@nuevo_estado";
                    nuevoEstadoParam.Value = nuevoEstado;
                    nuevoEstadoParam.DbType = DbType.Byte;
                    command.Parameters.Add(nuevoEstadoParam);

                    // Parámetro de salida: Mensaje del procedimiento almacenado
                    var mensajeParam = command.CreateParameter();
                    mensajeParam.ParameterName = "@p_mensaje";
                    mensajeParam.DbType = DbType.String;
                    mensajeParam.Size = 4000;
                    mensajeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    await command.ExecuteNonQueryAsync();

                    // Obtener mensaje del SP
                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al cambiar el estado del usuario.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cambiar estado del usuario con ID {idUsuario}: {ex.Message}");
                mensaje = "Error inesperado al cambiar el estado del usuario.";
            }

            return mensaje;
        }

    }
}
