namespace app_fianzas.Models
{
    public class EmpresaRegistroModel
    {
        public int TipoEmpresaId { get; set; }
        public int EmpresaId { get; set; }
        public string NombreEmpresa { get; set; }
        public string DireccionEmpresa { get; set; }
        public string CiEmpresa { get; set; }
        public string TelefonoEmpresa { get; set; }
        public string EmailEmpresa { get; set; }
        public byte? EstadoEmpresa { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public decimal ActivoCorriente { get; set; }
        public decimal ActivoFijo { get; set; }
        public decimal Capital { get; set; }
        public decimal Reserva { get; set; }
        public decimal Perdida { get; set; }
        public decimal Ventas { get; set; }
        public decimal Utilidad { get; set; }
        public decimal CupoTotal { get; set; }


        public decimal HistorialActivoC { get; set; }

        public decimal HistorialActivoF { get; set; }

        public decimal HistorialCapital { get; set; }

        public decimal HistorialReserva { get; set; }

        public decimal HistorialPerdida { get; set; }

        public decimal HistorialVentas { get; set; }

        public decimal HistorialUtilidad { get; set; }

        public decimal HistorialCupoAsignado { get; set; }

        public decimal? HistorialCupoRestante { get; set; }

        public string? HistorialOperacion { get; set; }

    }
}
