using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class EmpresaFinanza
{
    public int EmpresaFinanzasId { get; set; }

    public int EmpresaId { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public decimal ActivoCorriente { get; set; }

    public decimal ActivoFijo { get; set; }

    public decimal Capital { get; set; }

    public decimal Reserva { get; set; }

    public decimal Perdida { get; set; }

    public decimal Ventas { get; set; }

    public decimal Utilidad { get; set; }

    public decimal CupoTotal { get; set; }

    public virtual Empresa Empresa { get; set; } = null!;
}
