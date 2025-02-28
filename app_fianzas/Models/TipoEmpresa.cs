using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class TipoEmpresa
{
    public int TipoEmpresaId { get; set; }

    public string NombreTipo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public byte? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();
}
