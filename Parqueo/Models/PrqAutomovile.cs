using System;
using System.Collections.Generic;

namespace Parqueo.Models;

public partial class PrqAutomovile
{
    public int IdAutomovil { get; set; }

    public string Color { get; set; } = null!;

    public int Anio { get; set; }

    public string Fabricante { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public virtual ICollection<PrqIngresoAutomovile> PrqIngresoAutomoviles { get; set; } = new List<PrqIngresoAutomovile>();
}
