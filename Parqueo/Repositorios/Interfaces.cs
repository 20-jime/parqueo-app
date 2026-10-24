using Parqueo.Models;

namespace Parqueo.Repositorios;

// Resultado de las consultas de ingresos: hora de entrada, de salida y monto a cancelar
public record IngresoDetalle(
    int Consecutivo,
    string TipoAutomovil,
    string Color,
    string Fabricante,
    string NombreParqueo,
    string NombreProvincia,
    DateTime FechaHoraEntrada,
    DateTime? FechaHoraSalida,
    decimal? MontoTotal)
{
    // Arma el detalle con el precio por hora que devolvió ObtenerPrecioPorHoraPorParqueo
    public static IngresoDetalle Crear(
        PrqIngresoAutomovile ingreso, PrqAutomovile automovil, PrqParqueo parqueo, decimal? precioPorHora)
    {
        ingreso.PrecioPorHoraAplicado = precioPorHora;
        return new IngresoDetalle(
            ingreso.Consecutivo, automovil.Tipo, automovil.Color, automovil.Fabricante,
            parqueo.Nombre, parqueo.NombreProvincia,
            ingreso.FechaHoraEntrada, ingreso.FechaHoraSalida, ingreso.MontoTotal);
    }
}

public interface IAutomovilRepositorio
{
    PrqAutomovile? ObtenerPorId(int idAutomovil);
    List<PrqAutomovile> ObtenerTodos();
    List<PrqAutomovile> Buscar(string? color = null, int? anioDesde = null, int? anioHasta = null,
                               string? fabricante = null, string? tipo = null);
}

public interface IParqueoRepositorio
{
    PrqParqueo? ObtenerPorId(int idParqueo);
    List<PrqParqueo> ObtenerTodos();
    List<PrqParqueo> Buscar(string? provincia = null, string? nombre = null,
                            decimal? precioMin = null, decimal? precioMax = null);
}

public interface IIngresoRepositorio
{
    PrqIngresoAutomovile? ObtenerPorId(int consecutivo);
    List<PrqIngresoAutomovile> ObtenerTodos();
    decimal? ObtenerPrecioPorHoraPorParqueo(int idParqueo);
    List<IngresoDetalle> ListarPorTipo(string tipo, DateTime desde, DateTime hasta);
    List<IngresoDetalle> ListarPorProvincia(string provincia, DateTime desde, DateTime hasta);
}