using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class Prendum
{
    public int PrendaId { get; set; }

    public string? PrendaTipo { get; set; }

    public string? PrendaBien { get; set; }

    public string? PrendaDescripcion { get; set; }

    public decimal? PrendaValor { get; set; }

    public string? PrendaUbicacion { get; set; }

    public string? PrendaCustodio { get; set; }

    public DateOnly? PrendaFechaConstatacion { get; set; }

    public string? PrendaResponsableConstatacion { get; set; }

    public byte[]? PrendaArchivo { get; set; }

    public byte? PrendaEstado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();

    public virtual ICollection<SolicitudFianza> SolicitudFianzasNavigation { get; set; } = new List<SolicitudFianza>();
}
