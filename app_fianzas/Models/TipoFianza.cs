using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class TipoFianza
{
    public int TipoFianzaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string? CodigoFianza { get; set; }

    public string? Requisitos { get; set; }

    public byte? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();
}
