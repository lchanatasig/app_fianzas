using app_fianzas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace app_fianzas.Servicios
{
    public class EmpresaService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EmpresaService> _logger;
        private readonly AppFianzaUnidosContext _dbContext;

        public EmpresaService(IHttpContextAccessor httpContextAccessor, ILogger<EmpresaService> logger, AppFianzaUnidosContext dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Funcion que carga el sp desde la base
        /// </summary>
        /// <param name="tipoEmpresaId"></param>
        /// <param name="nombreEmpresa"></param>
        /// <param name="direccionEmpresa"></param>
        /// <param name="ciEmpresa"></param>
        /// <param name="telefonoEmpresa"></param>
        /// <param name="emailEmpresa"></param>
        /// <param name="activoCorriente"></param>
        /// <param name="activoFijo"></param>
        /// <param name="capital"></param>
        /// <param name="reserva"></param>
        /// <param name="perdida"></param>
        /// <param name="ventas"></param>
        /// <param name="utilidad"></param>
        /// <param name="cupoTotal"></param>
        /// <returns></returns>
        public async Task<(int? IdEmpresa, string Mensaje)> InsertarEmpresaAsync(
           int tipoEmpresaId, string nombreEmpresa, string direccionEmpresa, string ciEmpresa,
           string telefonoEmpresa, string emailEmpresa, decimal activoCorriente, decimal activoFijo,
           decimal capital, decimal reserva, decimal perdida, decimal ventas, decimal utilidad, decimal cupoTotal)
        {
            int? nuevoIdEmpresa = null;
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_insert_empresa";
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros de entrada
                    command.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@tipo_empresa_id", tipoEmpresaId) { DbType = DbType.Int32 },
                        new SqlParameter("@nombre_empresa", nombreEmpresa) { DbType = DbType.String },
                        new SqlParameter("@direccion_empresa", direccionEmpresa) { DbType = DbType.String },
                        new SqlParameter("@ci_empresa", ciEmpresa) { DbType = DbType.String },
                        new SqlParameter("@telefono_empresa", telefonoEmpresa) { DbType = DbType.String },
                        new SqlParameter("@email_empresa", emailEmpresa) { DbType = DbType.String },
                        new SqlParameter("@activo_corriente", activoCorriente) { DbType = DbType.Decimal },
                        new SqlParameter("@activo_fijo", activoFijo) { DbType = DbType.Decimal },
                        new SqlParameter("@capital", capital) { DbType = DbType.Decimal },
                        new SqlParameter("@reserva", reserva) { DbType = DbType.Decimal },
                        new SqlParameter("@perdida", perdida) { DbType = DbType.Decimal },
                        new SqlParameter("@ventas", ventas) { DbType = DbType.Decimal },
                        new SqlParameter("@utilidad", utilidad) { DbType = DbType.Decimal },
                        new SqlParameter("@cupo_total", cupoTotal) { DbType = DbType.Decimal }
                    });

                    // Parámetro de salida para mensaje
                    var mensajeParam = new SqlParameter("@p_mensaje", SqlDbType.NVarChar, 4000)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync() && !reader.IsDBNull(reader.GetOrdinal("nuevo_id_empresa")))
                        {
                            nuevoIdEmpresa = reader.GetInt32(reader.GetOrdinal("nuevo_id_empresa"));
                        }
                    }

                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al insertar la empresa.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al insertar empresa: {ex.Message}");
                mensaje = "Error inesperado al insertar la empresa.";
            }

            return (nuevoIdEmpresa, mensaje);
        }

        /// <summary>
        ///  Listar todas las empresas desde el sp 
        /// </summary>
        /// <returns></returns>

        public async Task<(List<Empresa> Empresas, string Mensaje)> ListarEmpresasAsync()
        {
            List<Empresa> listaEmpresas = new List<Empresa>();
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_listar_empresas";
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
                            var empresa = new Empresa
                            {
                                EmpresaId = reader.GetInt32(reader.GetOrdinal("empresa_id")),
                                TipoEmpresaId = reader.GetInt32(reader.GetOrdinal("tipo_empresa_id")),
                                NombreEmpresa = reader.GetString(reader.GetOrdinal("nombre_empresa")),
                                DireccionEmpresa = reader.GetString(reader.GetOrdinal("direccion_empresa")),
                                CiEmpresa = reader.GetString(reader.GetOrdinal("ci_empresa")),
                                TelefonoEmpresa = reader.GetString(reader.GetOrdinal("telefono_empresa")),
                                EmailEmpresa = reader.GetString(reader.GetOrdinal("email_empresa")),
                                EstadoEmpresa = reader.GetByte(reader.GetOrdinal("estado_empresa")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("fecha_actualizacion")),
                                TipoEmpresa = new TipoEmpresa
                                {
                                    NombreTipo = reader.GetString(reader.GetOrdinal("nombre_tipo"))
                                },
                                // ✅ CORRECTO: Crear una lista con un solo elemento en lugar de asignar un solo objeto
                                EmpresaFinanzas = new List<EmpresaFinanza>
            {
                new EmpresaFinanza
                {
                    ActivoCorriente = reader.GetDecimal(reader.GetOrdinal("activo_corriente")),
                    ActivoFijo = reader.GetDecimal(reader.GetOrdinal("activo_fijo")),
                    Capital = reader.GetDecimal(reader.GetOrdinal("capital")),
                    Reserva = reader.GetDecimal(reader.GetOrdinal("reserva")),
                    Perdida = reader.GetDecimal(reader.GetOrdinal("perdida")),
                    Ventas = reader.GetDecimal(reader.GetOrdinal("ventas")),
                    Utilidad = reader.GetDecimal(reader.GetOrdinal("utilidad")),
                    CupoTotal = reader.GetDecimal(reader.GetOrdinal("cupo_total"))
                }
            },
                                HistorialEmpresas = reader.IsDBNull(reader.GetOrdinal("fecha_historial"))
                                    ? new List<HistorialEmpresa>() // ✅ Devuelve una lista vacía en caso de ser nulo
                                    : new List<HistorialEmpresa>
                                    {
                    new HistorialEmpresa
                    {
                        HistorialActivoC = reader.GetDecimal(reader.GetOrdinal("historial_activoC")),
                        HistorialActivoF = reader.GetDecimal(reader.GetOrdinal("historial_activoF")),
                        HistorialCapital = reader.GetDecimal(reader.GetOrdinal("historial_capital")),
                        HistorialReserva = reader.GetDecimal(reader.GetOrdinal("historial_reserva")),
                        HistorialPerdida = reader.GetDecimal(reader.GetOrdinal("historial_perdida")),
                        HistorialVentas = reader.GetDecimal(reader.GetOrdinal("historial_ventas")),
                        HistorialUtilidad = reader.GetDecimal(reader.GetOrdinal("historial_utilidad")),
                        HistorialCupoAsignado = reader.GetDecimal(reader.GetOrdinal("historial_cupo_asignado")),
                        HistorialCupoRestante = reader.IsDBNull(reader.GetOrdinal("historial_cupo_restante"))
                            ? (decimal?)null
                            : reader.GetDecimal(reader.GetOrdinal("historial_cupo_restante")),
                        HistorialOperacion = reader.GetString(reader.GetOrdinal("historial_operacion")),
                        FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("fecha_historial"))
                    }
                                    }
                            };
                            listaEmpresas.Add(empresa);
                        }
                    }

                    mensaje = mensajeParam.Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al listar empresas: {ex.Message}");
                mensaje = "Error inesperado al listar las empresas.";
            }

            return (listaEmpresas, mensaje);
        }

        /// <summary>
        /// Servicio obtener empresa
        /// </summary>
        /// <param name="empresaId"></param>
        /// <returns></returns>
        public async Task<(EmpresaRegistroModel Empresa, string Mensaje)> ObtenerEmpresaPorIdAsync(int empresaId)
        {
            EmpresaRegistroModel empresa = null;
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_get_empresa";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada
                    var empresaIdParam = command.CreateParameter();
                    empresaIdParam.ParameterName = "@empresa_id";
                    empresaIdParam.Value = empresaId;
                    empresaIdParam.DbType = DbType.Int32;
                    command.Parameters.Add(empresaIdParam);

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
                            empresa = new EmpresaRegistroModel
                            {
                                EmpresaId = reader.GetInt32(reader.GetOrdinal("empresa_id")),
                                TipoEmpresaId = reader.GetInt32(reader.GetOrdinal("tipo_empresa_id")),
                                NombreEmpresa = reader.GetString(reader.GetOrdinal("nombre_empresa")),
                                DireccionEmpresa = reader.GetString(reader.GetOrdinal("direccion_empresa")),
                                CiEmpresa = reader.GetString(reader.GetOrdinal("ci_empresa")),
                                TelefonoEmpresa = reader.GetString(reader.GetOrdinal("telefono_empresa")),
                                EmailEmpresa = reader.GetString(reader.GetOrdinal("email_empresa")),
                                EstadoEmpresa = reader.GetByte(reader.GetOrdinal("estado_empresa")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("fecha_actualizacion")),


                                ActivoCorriente = reader.GetDecimal(reader.GetOrdinal("activo_corriente")),
                                ActivoFijo = reader.GetDecimal(reader.GetOrdinal("activo_fijo")),
                                Capital = reader.GetDecimal(reader.GetOrdinal("capital")),
                                Reserva = reader.GetDecimal(reader.GetOrdinal("reserva")),
                                Perdida = reader.GetDecimal(reader.GetOrdinal("perdida")),
                                Ventas = reader.GetDecimal(reader.GetOrdinal("ventas")),
                                Utilidad = reader.GetDecimal(reader.GetOrdinal("utilidad")),
                                CupoTotal = reader.GetDecimal(reader.GetOrdinal("cupo_total")),

                                HistorialActivoC = reader.GetDecimal(reader.GetOrdinal("historial_activoC")),
                                HistorialActivoF = reader.GetDecimal(reader.GetOrdinal("historial_activoF")),
                                HistorialCapital = reader.GetDecimal(reader.GetOrdinal("historial_capital")),
                                HistorialReserva = reader.GetDecimal(reader.GetOrdinal("historial_reserva")),
                                HistorialPerdida = reader.GetDecimal(reader.GetOrdinal("historial_perdida")),
                                HistorialVentas = reader.GetDecimal(reader.GetOrdinal("historial_ventas")),
                                HistorialUtilidad = reader.GetDecimal(reader.GetOrdinal("historial_utilidad")),
                                HistorialCupoAsignado = reader.GetDecimal(reader.GetOrdinal("historial_cupo_asignado")),
                                HistorialCupoRestante = reader.IsDBNull(reader.GetOrdinal("historial_cupo_restante"))
                                        ? (decimal?)null
                                        : reader.GetDecimal(reader.GetOrdinal("historial_cupo_restante")),
                                HistorialOperacion = reader.GetString(reader.GetOrdinal("historial_operacion"))
                            };
                        }
                    }

                    mensaje = mensajeParam.Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener empresa con ID {empresaId}: {ex.Message}");
                mensaje = "Error inesperado al obtener la empresa.";
            }

            return (empresa, mensaje);
        }

        /// <summary>
        /// Actualizacion del sistema
        /// </summary>
        /// <param name="empresaId"></param>
        /// <param name="tipoEmpresaId"></param>
        /// <param name="nombreEmpresa"></param>
        /// <param name="direccionEmpresa"></param>
        /// <param name="ciEmpresa"></param>
        /// <param name="telefonoEmpresa"></param>
        /// <param name="emailEmpresa"></param>
        /// <param name="activoCorriente"></param>
        /// <param name="activoFijo"></param>
        /// <param name="capital"></param>
        /// <param name="reserva"></param>
        /// <param name="perdida"></param>
        /// <param name="ventas"></param>
        /// <param name="utilidad"></param>
        /// <param name="cupoTotal"></param>
        /// <returns></returns>
        public async Task<string> ActualizarEmpresaAsync(
          int empresaId, int tipoEmpresaId, string nombreEmpresa, string direccionEmpresa, string ciEmpresa,
          string telefonoEmpresa, string emailEmpresa, decimal activoCorriente, decimal activoFijo,
          decimal capital, decimal reserva, decimal perdida, decimal ventas, decimal utilidad, decimal cupoTotal)
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
                    command.CommandText = "sp_update_empresa";
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros de entrada
                    command.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@empresa_id", empresaId) { DbType = DbType.Int32 },
                        new SqlParameter("@tipo_empresa_id", tipoEmpresaId) { DbType = DbType.Int32 },
                        new SqlParameter("@nombre_empresa", nombreEmpresa) { DbType = DbType.String },
                        new SqlParameter("@direccion_empresa", direccionEmpresa) { DbType = DbType.String },
                        new SqlParameter("@ci_empresa", ciEmpresa) { DbType = DbType.String },
                        new SqlParameter("@telefono_empresa", telefonoEmpresa) { DbType = DbType.String },
                        new SqlParameter("@email_empresa", emailEmpresa) { DbType = DbType.String },
                        new SqlParameter("@activo_corriente", activoCorriente) { DbType = DbType.Decimal },
                        new SqlParameter("@activo_fijo", activoFijo) { DbType = DbType.Decimal },
                        new SqlParameter("@capital", capital) { DbType = DbType.Decimal },
                        new SqlParameter("@reserva", reserva) { DbType = DbType.Decimal },
                        new SqlParameter("@perdida", perdida) { DbType = DbType.Decimal },
                        new SqlParameter("@ventas", ventas) { DbType = DbType.Decimal },
                        new SqlParameter("@utilidad", utilidad) { DbType = DbType.Decimal },
                        new SqlParameter("@cupo_total", cupoTotal) { DbType = DbType.Decimal }
                    });

                    // Parámetro de salida para mensaje
                    var mensajeParam = new SqlParameter("@p_mensaje", SqlDbType.NVarChar, 4000)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    // Ejecutar el procedimiento almacenado
                    await command.ExecuteNonQueryAsync();

                    // Obtener mensaje del SP
                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al actualizar la empresa.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar empresa con ID {empresaId}: {ex.Message}");
                mensaje = "Error inesperado al actualizar la empresa.";
            }

            return mensaje;
        }

        /// <summary>
        /// Obtener el Hisotrico
        /// </summary>
        /// <param name="empresaId"></param>
        /// <returns></returns>
        public async Task<(List<HistorialEmpresa> Historial, string Mensaje)> ObtenerHistorialEmpresaAsync(int empresaId)
        {
            List<HistorialEmpresa> listaHistorial = new List<HistorialEmpresa>();
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_get_historial_empresa";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada
                    var empresaIdParam = command.CreateParameter();
                    empresaIdParam.ParameterName = "@empresa_id";
                    empresaIdParam.Value = empresaId;
                    empresaIdParam.DbType = DbType.Int32;
                    command.Parameters.Add(empresaIdParam);

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
                        while (await reader.ReadAsync())
                        {
                            var historial = new HistorialEmpresa
                            {
                                HistorialId = reader.GetInt32(reader.GetOrdinal("historial_id")),
                                EmpresaId = reader.GetInt32(reader.GetOrdinal("empresa_id")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("fecha_actualizacion")),
                                HistorialActivoC = reader.GetDecimal(reader.GetOrdinal("historial_activoC")),
                                HistorialActivoF = reader.GetDecimal(reader.GetOrdinal("historial_activoF")),
                                HistorialCapital = reader.GetDecimal(reader.GetOrdinal("historial_capital")),
                                HistorialReserva = reader.GetDecimal(reader.GetOrdinal("historial_reserva")),
                                HistorialPerdida = reader.GetDecimal(reader.GetOrdinal("historial_perdida")),
                                HistorialVentas = reader.GetDecimal(reader.GetOrdinal("historial_ventas")),
                                HistorialUtilidad = reader.GetDecimal(reader.GetOrdinal("historial_utilidad")),
                                HistorialCupoAsignado = reader.GetDecimal(reader.GetOrdinal("historial_cupo_asignado")),
                                HistorialCupoRestante = reader.IsDBNull(reader.GetOrdinal("historial_cupo_restante"))
                                    ? (decimal?)null
                                    : reader.GetDecimal(reader.GetOrdinal("historial_cupo_restante")),
                                HistorialOperacion = reader.GetString(reader.GetOrdinal("historial_operacion"))
                            };
                            listaHistorial.Add(historial);
                        }
                    }

                    mensaje = mensajeParam.Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener historial de empresa con ID {empresaId}: {ex.Message}");
                mensaje = "Error inesperado al obtener el historial de la empresa.";
            }

            return (listaHistorial, mensaje);
        }

    }
}
