using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class Perfil
{
    public int IdPerfil { get; set; }

    public string NombrePerfil { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
