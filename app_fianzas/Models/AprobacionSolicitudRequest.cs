namespace app_fianzas.Models
{
    public class AprobacionSolicitudRequest
    {
        public int SolicitudFianzaId { get; set; }
        public bool? AprobacionTecnica { get; set; }
        public bool? AprobacionLegal { get; set; }
        public int UsuarioId { get; set; }
        public string Observaciones { get; set; }
    }

}
