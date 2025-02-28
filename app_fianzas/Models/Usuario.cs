using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdPerfil { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public byte? EstadoUsuario { get; set; }

    public virtual Perfil IdPerfilNavigation { get; set; } = null!;

    public virtual ICollection<SolicitudFianzaHistorial> SolicitudFianzaHistorials { get; set; } = new List<SolicitudFianzaHistorial>();
}
