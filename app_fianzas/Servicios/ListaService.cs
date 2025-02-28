using app_fianzas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace app_fianzas.Servicios
{
    public class ListaService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ListaService> _logger;
        private readonly AppFianzaUnidosContext _dbContext;
        private readonly UsuarioService _usuarioService;

        public ListaService(IHttpContextAccessor httpContextAccessor, ILogger<ListaService> logger, AppFianzaUnidosContext dbContext, UsuarioService usuarioService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Ejecuta el SP sp_listar_perfiles y retorna la lista de perfiles.
        /// </summary>
        /// <returns>Una lista de objetos Perfil.</returns>
        public async Task<List<Perfil>> ListarPerfilesAsync()
        {
            var perfiles = new List<Perfil>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_listar_perfiles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var perfil = new Perfil
                            {
                                IdPerfil = reader["id_perfil"] != DBNull.Value ? Convert.ToInt32(reader["id_perfil"]) : 0,
                                NombrePerfil = reader["nombre_perfil"].ToString(),
                            };

                            perfiles.Add(perfil);
                        }
                    }
                }
            }

            return perfiles;
        }

        /// <summary>
        /// Ejecuta el SP sp_listar_tipos_empresas y retorna la lista de tipos de empresas.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TipoEmpresa>> ListarTipoEmpresaAsync()
        {
            var tipoempresas = new List<TipoEmpresa>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_listar_tipos_empresas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var empresa = new TipoEmpresa
                            {
                                TipoEmpresaId = reader["tipo_empresa_id"] != DBNull.Value ? Convert.ToInt32(reader["tipo_empresa_id"]) : 0,
                                NombreTipo = reader["nombre_tipo"].ToString(),
                                Estado = reader["estado"] != DBNull.Value ? Convert.ToByte(reader["estado"]) : (byte)0
                            };

                            tipoempresas.Add(empresa);
                        }
                    }
                }
            }

            return tipoempresas;
        }
        /// <summary>
        /// Ejecuta el SP sp_listar_tipos_fianza y retorna la lista de tipos de fianza.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TipoFianza>> ListarTipoFianzasAsync()
        {
            var tipofianzas = new List<TipoFianza>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_listar_tipos_fianza", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tipoFianza = new TipoFianza
                            {
                                TipoFianzaId = reader["tipo_fianza_id"] != DBNull.Value ? Convert.ToInt32(reader["tipo_fianza_id"]) : 0,
                                Nombre = reader["nombre"].ToString(),
                                Estado = reader["estado"] != DBNull.Value ? Convert.ToByte(reader["estado"]) : (byte)0
                            };


                            tipofianzas.Add(tipoFianza);
                        }
                    }
                }
            }

            return tipofianzas;
        }


    }
}
