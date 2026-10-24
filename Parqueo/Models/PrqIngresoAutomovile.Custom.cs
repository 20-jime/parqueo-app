using System.ComponentModel.DataAnnotations.Schema;

namespace Parqueo.Models;

public partial class PrqIngresoAutomovile
{
    // Precio por hora del parqueo. Lo asigna el repositorio después de
    // llamar a ObtenerPrecioPorHoraPorParqueo.
    [NotMapped]
    public decimal? PrecioPorHoraAplicado { get; set; }

    // Duración de la estadía en minutos (nulo si aún no sale)
    [NotMapped]
    public double? DuracionMinutos
    {
        get
        {
            if (FechaHoraSalida is null) return null;
            return Math.Round((FechaHoraSalida.Value - FechaHoraEntrada).TotalMinutes, 2);
        }
    }

    // Duración de la estadía en horas (nulo si aún no sale)
    [NotMapped]
    public double? DuracionHoras
    {
        get
        {
            if (FechaHoraSalida is null) return null;
            return Math.Round((FechaHoraSalida.Value - FechaHoraEntrada).TotalHours, 2);
        }
    }

    // Monto total a pagar (nulo si no hay salida o no se conoce el precio)
    [NotMapped]
    public decimal? MontoTotal
    {
        get
        {
            if (FechaHoraSalida is null || PrecioPorHoraAplicado is null) return null;
            var horas = (decimal)(FechaHoraSalida.Value - FechaHoraEntrada).TotalHours;
            return Math.Round(horas * PrecioPorHoraAplicado.Value, 2);
        }
    }
}