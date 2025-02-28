using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class SolicitudFianzaHistorial
{
    public int HistorialId { get; set; }

    public int SolicitudFianzaId { get; set; }

    public int EstadoFianzaId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime? FechaCambio { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual EstadoFianza EstadoFianza { get; set; } = null!;

    public virtual SolicitudFianza SolicitudFianza { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
