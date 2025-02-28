using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class Beneficiario
{
    public int BeneficiarioId { get; set; }

    public string NombreBeneficiario { get; set; } = null!;

    public string DireccionBeneficiario { get; set; } = null!;

    public string CiRucBeneficiario { get; set; } = null!;

    public string EmailBeneficiario { get; set; } = null!;

    public string TelefonoBeneficiario { get; set; } = null!;

    public byte? EstadoBeneficiario { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<SolicitudFianza> SolicitudFianzas { get; set; } = new List<SolicitudFianza>();
}
