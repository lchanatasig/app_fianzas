using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class HistorialEmpresa
{
    public int HistorialId { get; set; }

    public int EmpresaId { get; set; }

    public DateTime? FechaActualizacion { get; set; }

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

    public virtual Empresa Empresa { get; set; } = null!;
}
