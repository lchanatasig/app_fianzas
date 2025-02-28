namespace app_fianzas.Models
{
    public class SolicitudFianzaRequest
    {
        public int EmpresaId { get; set; }
        public int TipoFianzaId { get; set; }
        public int EstadoFianzaId { get; set; }
        public DateTime FechaInicioVigencia { get; set; }
        public int PlazoGarantiaDias { get; set; }
        public string SectorFianza { get; set; }
        public decimal MontoFianza { get; set; }
        public decimal MontoContrato { get; set; }
        public decimal MontoGarantia { get; set; }
        public string ObjetoContrato { get; set; }
        public bool AprobacionTecnica { get; set; }
        public bool AprobacionLegal { get; set; }

        // Beneficiario
        public string NombreBeneficiario { get; set; }
        public string DireccionBeneficiario { get; set; }
        public string CiRucBeneficiario { get; set; }
        public string EmailBeneficiario { get; set; }
        public string TelefonoBeneficiario { get; set; }

        // Prenda (Opcional)
        public string PrendaTipo { get; set; }
        public string PrendaBien { get; set; }
        public string PrendaDescripcion { get; set; }
        public decimal? PrendaValor { get; set; }
        public string PrendaUbicacion { get; set; }
        public string PrendaCustodio { get; set; }
        public DateTime? PrendaFechaConstatacion { get; set; }
        public string PrendaResponsableConstatacion { get; set; }
        public byte[] PrendaArchivo { get; set; }

        // Lista de Prendas (Opcional)
        public List<Prendum> Prendas { get; set; }
        // Historial
        public int UsuarioId { get; set; }
        public string Observaciones { get; set; }
    }

}
