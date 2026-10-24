using System;
using System.Collections.Generic;

namespace Parqueo.Models;

public partial class PrqIngresoAutomovile
{
    public int Consecutivo { get; set; }

    public int IdParqueo { get; set; }

    public int IdAutomovil { get; set; }

    public DateTime FechaHoraEntrada { get; set; }

    public DateTime? FechaHoraSalida { get; set; }

    public virtual PrqAutomovile1 IdAutomovilNavigation { get; set; } = null!;

    public virtual PrqParqueo2 IdParqueoNavigation { get; set; } = null!;
}
