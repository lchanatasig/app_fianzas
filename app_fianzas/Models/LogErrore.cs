using System;
using System.Collections.Generic;

namespace app_fianzas.Models;

public partial class LogErrore
{
    public int LogId { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? ErrorDate { get; set; }
}
