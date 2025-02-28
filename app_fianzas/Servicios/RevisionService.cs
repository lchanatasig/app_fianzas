using app_fianzas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace app_fianzas.Servicios
{
    public class RevisionService
    {

        private readonly AppFianzaUnidosContext _dbContext;
        private readonly ILogger<RevisionService> _logger;
        private readonly FianzasService _fianzasService;

        public RevisionService(AppFianzaUnidosContext dbContext, ILogger<RevisionService> logger, FianzasService fianzasService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _fianzasService = fianzasService;
        }

        public async Task<string> AprobarSolicitudFianzaAsync(AprobacionSolicitudRequest request)
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
                    command.CommandText = "sp_aprobar_solicitud_fianza_mod";
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@solicitud_fianza_id", request.SolicitudFianzaId),
                        new SqlParameter("@aprobacion_tecnica", request.AprobacionTecnica ?? (object)DBNull.Value),
                        new SqlParameter("@aprobacion_legal", request.AprobacionLegal ?? (object)DBNull.Value),
                        new SqlParameter("@usuario_id", request.UsuarioId),
                        new SqlParameter("@observaciones", request.Observaciones ?? (object)DBNull.Value)
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
                    mensaje = mensajeParam.Value?.ToString() ?? "Error inesperado al aprobar la solicitud.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al aprobar solicitud de fianza ID {request.SolicitudFianzaId}: {ex.Message}");
                mensaje = "Error inesperado al aprobar la solicitud.";
            }

            return mensaje;
        }

    }
}
