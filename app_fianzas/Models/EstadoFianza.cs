using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class EstadoFianza
{
    public int EstadoFianzaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<SolicitudFianzaHistorial> SolicitudFianzaHistorials { get; set; } = new List<SolicitudFianzaHistorial>();

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();
}
