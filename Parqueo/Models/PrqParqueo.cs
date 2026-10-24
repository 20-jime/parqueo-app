using System;
using System.Collections.Generic;

namespace Parqueo.Models;

public partial class PrqParqueo
{
    public int IdParqueo { get; set; }

    public string NombreProvincia { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public decimal PrecioPorHora { get; set; }

    public virtual ICollection<PrqIngresoAutomovile> PrqIngresoAutomoviles { get; set; } = new List<PrqIngresoAutomovile>();
}
