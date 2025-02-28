using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class SolicitudFianza
{
    public int SolicitudFianzaId { get; set; }

    public int EmpresaId { get; set; }

    public int TipoFianzaId { get; set; }

    public int EstadoFianzaId { get; set; }

    public int BeneficiarioId { get; set; }

    public int? PrendaId { get; set; }

    public DateTime? FechaSolicitud { get; set; }

    public DateOnly FechaInicioVigencia { get; set; }

    public int PlazoGarantiaDias { get; set; }

    public string? SectorFianza { get; set; }

    public decimal MontoFianza { get; set; }

    public decimal MontoContrato { get; set; }

    public decimal MontoGarantia { get; set; }

    public string ObjetoContrato { get; set; } = null!;

    public bool? AprobacionTecnica { get; set; }

    public bool? AprobacionLegal { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Beneficiario Beneficiario { get; set; } = null!;

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual EstadoFianza EstadoFianza { get; set; } = null!;

    public virtual Prendum? Prenda { get; set; }

    public virtual ICollection<SolicitudFianzaHistorial> SolicitudFianzaHistorials { get; set; } = new List<SolicitudFianzaHistorial>();

    public virtual TipoFianza TipoFianza { get; set; } = null!;

    public virtual ICollection<Prendum> PrendaNavigation { get; set; } = new List<Prendum>();
}
