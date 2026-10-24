using System;
using System.Collections.Generic;

namespace Parqueo.Models;

/// <summary>
/// Parqueos (estacionamientos) disponibles.
/// </summary>
public partial class PrqParqueo1
{
    /// <summary>
    /// Llave primaria del parqueo.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Provincia de Costa Rica donde se ubica el parqueo.
    /// </summary>
    public string NombreProvincia { get; set; } = null!;

    /// <summary>
    /// Nombre del parqueo.
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Tarifa por hora en colones.
    /// </summary>
    public decimal PrecioPorHora { get; set; }
}
