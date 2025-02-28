using app_fianzas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace app_fianzas.Servicios
{
    public class DocumentosService
    { 
        private AppFianzaUnidosContext _dbContext;
        private ILogger<DocumentosService> _logger;
        public DocumentosService (AppFianzaUnidosContext dbContext,ILogger<DocumentosService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> InsertarDocumentacionYAprobarAsync(SolicitudFianzaDocumentoRequest request)
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
                    command.CommandText = "sp_insert_documentacion_y_aprobar_solicitud_fianza";
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@solicitud_fianza_id", request.SolicitudFianzaId),
                        new SqlParameter("@documento_solicitud", request.DocumentoSolicitud ?? (object)DBNull.Value),
                        new SqlParameter("@documento_convenio", request.DocumentoConvenio ?? (object)DBNull.Value),
                        new SqlParameter("@documento_pagare", request.DocumentoPagare ?? (object)DBNull.Value),
                        new SqlParameter("@documento_prenda", request.DocumentoPrenda ?? (object)DBNull.Value),
                        new SqlParameter("@fecha_subida", request.FechaSubida ?? (object)DBNull.Value),
                        new SqlParameter("@fecha_vencimiento", request.FechaVencimiento ?? (object)DBNull.Value),
                        new SqlParameter("@poliza", request.Poliza ?? (object)DBNull.Value),
                        new SqlParameter("@usuario_id", request.UsuarioId)
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
                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al insertar la documentación y aprobar la solicitud.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al insertar documentación y aprobar solicitud de fianza ID {request.SolicitudFianzaId}: {ex.Message}");
                mensaje = "Error inesperado al insertar la documentación y aprobar la solicitud.";
            }

            return mensaje;
        }
    }

}
