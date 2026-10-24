using System;
using System.Collections.Generic;

namespace Parqueo.Models;

public partial class PrqAutomovile
{
    public int Id { get; set; }

    public string Color { get; set; } = null!;

    public short Anio { get; set; }

    public string Fabricante { get; set; } = null!;

    public string Tipo { get; set; } = null!;
}
