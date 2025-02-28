using app_fianzas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace app_fianzas.Servicios
{
    public class FianzasService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FianzasService> _logger;
        private readonly AppFianzaUnidosContext _dbContext;


        public FianzasService(IHttpContextAccessor httpContextAccessor, ILogger<FianzasService> logger, AppFianzaUnidosContext dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<(int? IdSolicitud, string Mensaje)> InsertarSolicitudFianzaAsync(SolicitudFianzaRequest request)
        {
            int? nuevaSolicitudId = null;
            string mensaje = "";

            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_insert_solicitud_fianza";
                    command.CommandType = CommandType.StoredProcedure;

                    // Crear tabla de tipo para prendas
                    var prendaTable = new DataTable();
                    prendaTable.Columns.Add("prenda_tipo", typeof(string));
                    prendaTable.Columns.Add("prenda_bien", typeof(string));
                    prendaTable.Columns.Add("prenda_descripcion", typeof(string));
                    prendaTable.Columns.Add("prenda_valor", typeof(decimal));
                    prendaTable.Columns.Add("prenda_ubicacion", typeof(string));
                    prendaTable.Columns.Add("prenda_custodio", typeof(string));
                    prendaTable.Columns.Add("prenda_fecha_constatacion", typeof(DateTime)); // Asegurar que sea DateTime
                    prendaTable.Columns.Add("prenda_responsable_constatacion", typeof(string));
                    prendaTable.Columns.Add("prenda_archivo", typeof(byte[]));

                    if (request.Prendas != null)
                    {
                        foreach (var prenda in request.Prendas)
                        {
                            prendaTable.Rows.Add(
                                prenda.PrendaTipo,
                                prenda.PrendaBien,
                                prenda.PrendaDescripcion,
                                prenda.PrendaValor,
                                prenda.PrendaUbicacion,
                                prenda.PrendaCustodio,
                                prenda.PrendaFechaConstatacion.HasValue
                                    ? prenda.PrendaFechaConstatacion.Value.ToDateTime(TimeOnly.MinValue)
                                    : DBNull.Value, // ✅ Convertir DateOnly a DateTime
                                prenda.PrendaResponsableConstatacion,
                                prenda.PrendaArchivo);
                        }
                    }

                    // Agregar parámetros
                    command.Parameters.AddRange(new[]
                    {
                new SqlParameter("@empresa_id", request.EmpresaId),
                new SqlParameter("@tipo_fianza_id", request.TipoFianzaId),
                new SqlParameter("@estado_fianza_id", request.EstadoFianzaId),
           new SqlParameter("@fecha_inicio_vigencia", SqlDbType.DateTime)
{
    Value = request.FechaInicioVigencia != DateTime.MinValue
        ? request.FechaInicioVigencia
        : DateTime.Now.Date // ✅ Si es inválido, usa la fecha del día actual
},


                new SqlParameter("@plazo_garantia_dias", request.PlazoGarantiaDias),
                new SqlParameter("@sector_fianza", request.SectorFianza ?? (object)DBNull.Value),
                new SqlParameter("@monto_fianza", request.MontoFianza),
                new SqlParameter("@monto_contrato", request.MontoContrato),
                new SqlParameter("@monto_garantia", request.MontoGarantia),
                new SqlParameter("@objeto_contrato", request.ObjetoContrato),
                new SqlParameter("@aprobacion_tecnica", request.AprobacionTecnica),
                new SqlParameter("@aprobacion_legal", request.AprobacionLegal),

                // Datos del Beneficiario
                new SqlParameter("@nombre_beneficiario", request.NombreBeneficiario),
                new SqlParameter("@direccion_beneficiario", request.DireccionBeneficiario),
                new SqlParameter("@ci_ruc_beneficiario", request.CiRucBeneficiario),
                new SqlParameter("@email_beneficiario", request.EmailBeneficiario),
                new SqlParameter("@telefono_beneficiario", request.TelefonoBeneficiario),

                // Parámetro tipo tabla para prendas
                new SqlParameter("@Prendas", SqlDbType.Structured)
                {
                    TypeName = "dbo.PrendaType",
                    Value = prendaTable
                },

                // Datos de Historial
                new SqlParameter("@usuario_id", request.UsuarioId),
                new SqlParameter("@observaciones", request.Observaciones ?? "Primer registro")
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
                        if (await reader.ReadAsync() && !reader.IsDBNull(reader.GetOrdinal("nuevo_id_solicitud")))
                        {
                            nuevaSolicitudId = reader.GetInt32(reader.GetOrdinal("nuevo_id_solicitud"));
                        }
                    }

                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al insertar la solicitud de fianza.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al insertar solicitud de fianza: {ex.Message}");
                mensaje = "Error inesperado al insertar la solicitud de fianza.";
            }

            return (nuevaSolicitudId, mensaje);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(List<SolicitudFianza> Solicitudes, string Mensaje)> ListarSolicitudesFianzaAsync()
        {
            List<SolicitudFianza> listaSolicitudes = new List<SolicitudFianza>();
            string mensaje = "";

            // Obtener la conexión desde el DbContext
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_listar_solicitud_fianzas";
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
                            var solicitud = new SolicitudFianza
                            {
                                SolicitudFianzaId = reader.GetInt32(reader.GetOrdinal("solicitud_fianza_id")),
                                EmpresaId = reader.GetInt32(reader.GetOrdinal("empresa_id")),
                                TipoFianzaId = reader.GetInt32(reader.GetOrdinal("tipo_fianza_id")),
                                EstadoFianzaId = reader.GetInt32(reader.GetOrdinal("estado_fianza_id")),
                                FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("fecha_solicitud")),
                                FechaInicioVigencia = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fecha_inicio_vigencia"))),
                                PlazoGarantiaDias = reader.GetInt32(reader.GetOrdinal("plazo_garantia_dias")),
                                SectorFianza = reader.GetString(reader.GetOrdinal("sector_fianza")),
                                MontoFianza = reader.GetDecimal(reader.GetOrdinal("monto_fianza")),
                                MontoContrato = reader.GetDecimal(reader.GetOrdinal("monto_contrato")),
                                MontoGarantia = reader.GetDecimal(reader.GetOrdinal("monto_garantia")),
                                ObjetoContrato = reader.GetString(reader.GetOrdinal("objeto_contrato")),
                                AprobacionTecnica = reader.GetBoolean(reader.GetOrdinal("aprobacion_tecnica")),
                                AprobacionLegal = reader.GetBoolean(reader.GetOrdinal("aprobacion_legal")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("fecha_actualizacion")),

                                // Datos de la empresa
                                Empresa = new Empresa
                                {
                                    NombreEmpresa = reader.GetString(reader.GetOrdinal("nombre_empresa")),
                                    DireccionEmpresa = reader.GetString(reader.GetOrdinal("direccion_empresa")),
                                    CiEmpresa = reader.GetString(reader.GetOrdinal("ci_empresa")),
                                    TelefonoEmpresa = reader.GetString(reader.GetOrdinal("telefono_empresa")),
                                    EmailEmpresa = reader.GetString(reader.GetOrdinal("email_empresa")),
                                    HistorialEmpresas = new List<HistorialEmpresa>() // Inicializa la lista vacía
                                },

                                // Datos del tipo de fianza
                                TipoFianza = new TipoFianza
                                {
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("tipo_fianza_descripcion"))
                                },

                                // Datos del estado de fianza
                                EstadoFianza = new EstadoFianza
                                {
                                    Nombre = reader.GetString(reader.GetOrdinal("estado_fianza_nombre")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("estado_fianza_descripcion"))
                                },

                                // Datos del beneficiario
                                Beneficiario = new Beneficiario
                                {
                                    NombreBeneficiario = reader.GetString(reader.GetOrdinal("nombre_beneficiario")),
                                    DireccionBeneficiario = reader.GetString(reader.GetOrdinal("direccion_beneficiario")),
                                    CiRucBeneficiario = reader.GetString(reader.GetOrdinal("ci_ruc_beneficiario")),
                                    EmailBeneficiario = reader.GetString(reader.GetOrdinal("email_beneficiario")),
                                    TelefonoBeneficiario = reader.GetString(reader.GetOrdinal("telefono_beneficiario"))
                                },

                                // Datos de la prenda (si existe)
                                Prenda = reader.IsDBNull(reader.GetOrdinal("prenda_id")) ? null : new Prendum
                                {
                                    PrendaId = reader.GetInt32(reader.GetOrdinal("prenda_id")),
                                    PrendaTipo = reader.GetString(reader.GetOrdinal("prenda_tipo")),
                                    PrendaBien = reader.GetString(reader.GetOrdinal("prenda_bien")),
                                    PrendaDescripcion = reader.GetString(reader.GetOrdinal("prenda_descripcion")),
                                    PrendaValor = reader.GetDecimal(reader.GetOrdinal("prenda_valor")),
                                    PrendaUbicacion = reader.GetString(reader.GetOrdinal("prenda_ubicacion")),
                                    PrendaCustodio = reader.GetString(reader.GetOrdinal("prenda_custodio")),
                                    PrendaFechaConstatacion = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("prenda_fecha_constatacion"))),
                                    PrendaResponsableConstatacion = reader.GetString(reader.GetOrdinal("prenda_responsable_constatacion"))
                                }
                            };

                            // Agregar el historial de la empresa dentro de la empresa
                            if (!reader.IsDBNull(reader.GetOrdinal("historial_fecha_actualizacion")))
                            {
                                solicitud.Empresa.HistorialEmpresas.Add(new HistorialEmpresa
                                {
                                    HistorialCupoAsignado = reader.GetDecimal(reader.GetOrdinal("historial_cupo_asignado")),
                                    HistorialCupoRestante = reader.GetDecimal(reader.GetOrdinal("historial_cupo_restante")),
                                    HistorialOperacion = reader.GetString(reader.GetOrdinal("historial_operacion")),
                                    FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("historial_fecha_actualizacion"))
                                });
                            }

                            listaSolicitudes.Add(solicitud);

                        }
                    }

                    mensaje = mensajeParam.Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al listar solicitudes de fianza: {ex.Message}");
                mensaje = "Error inesperado al listar las solicitudes de fianza.";
            }

            return (listaSolicitudes, mensaje);
        }

    }
}
