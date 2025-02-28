using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class Empresa
{
    public int EmpresaId { get; set; }

    public int TipoEmpresaId { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public string DireccionEmpresa { get; set; } = null!;

    public string CiEmpresa { get; set; } = null!;

    public string TelefonoEmpresa { get; set; } = null!;

    public string EmailEmpresa { get; set; } = null!;

    public byte? EstadoEmpresa { get; set; }

    public virtual ICollection<EmpresaFinanza> EmpresaFinanzas { get; set; } = new List<EmpresaFinanza>();

    public virtual ICollection<HistorialEmpresa> HistorialEmpresas { get; set; } = new List<HistorialEmpresa>();

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();

    public virtual TipoEmpresa TipoEmpresa { get; set; } = null!;
}
