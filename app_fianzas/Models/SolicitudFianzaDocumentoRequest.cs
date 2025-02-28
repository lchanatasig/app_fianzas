namespace app_fianzas.Models
{
    public class SolicitudFianzaDocumentoRequest
    {
        public int SolicitudFianzaId { get; set; }
        public byte[] DocumentoSolicitud { get; set; }
        public byte[] DocumentoConvenio { get; set; }
        public byte[] DocumentoPagare { get; set; }
        public byte[] DocumentoPrenda { get; set; }
        public DateTime? FechaSubida { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Poliza { get; set; }
        public int UsuarioId { get; set; }
    }
}
