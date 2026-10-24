using System.Text.Json;
using Parqueo.Models;

namespace Parqueo.Repositorios;

// Lee los archivos de la carpeta Json, sin importar desde dónde se ejecute el programa
public static class JsonAlmacen
{
    private static readonly JsonSerializerOptions Opciones = new() { PropertyNameCaseInsensitive = true };

    public static string CarpetaJson()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidata = Path.Combine(dir.FullName, "Json");
            if (Directory.Exists(candidata)) return candidata;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("No se encontró la carpeta Json.");
    }

    public static List<T> Leer<T>(string archivo)
    {
        var texto = File.ReadAllText(Path.Combine(CarpetaJson(), archivo));
        return JsonSerializer.Deserialize<List<T>>(texto, Opciones) ?? new List<T>();
    }
}

internal static class Texto
{
    // Búsqueda aproximada: si no hay filtro, todo coincide
    public static bool Contiene(string? texto, string? filtro) =>
        string.IsNullOrWhiteSpace(filtro)
        || (texto?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false);
}

public class AutomovilJsonRepositorio : IAutomovilRepositorio
{
    private static List<PrqAutomovile> Cargar() => JsonAlmacen.Leer<PrqAutomovile>("automoviles.json");

    public PrqAutomovile? ObtenerPorId(int idAutomovil) =>
        Cargar().FirstOrDefault(a => a.IdAutomovil == idAutomovil);

    public List<PrqAutomovile> ObtenerTodos() => Cargar().OrderBy(a => a.IdAutomovil).ToList();

    public List<PrqAutomovile> Buscar(string? color = null, int? anioDesde = null, int? anioHasta = null,
                                      string? fabricante = null, string? tipo = null) =>
        Cargar().Where(a =>
                Texto.Contiene(a.Color, color)
                && (anioDesde is null || a.Anio >= anioDesde)
                && (anioHasta is null || a.Anio <= anioHasta)
                && Texto.Contiene(a.Fabricante, fabricante)
                && Texto.Contiene(a.Tipo, tipo))
            .OrderBy(a => a.IdAutomovil).ToList();
}

public class ParqueoJsonRepositorio : IParqueoRepositorio
{
    private static List<PrqParqueo> Cargar() => JsonAlmacen.Leer<PrqParqueo>("parqueos.json");

    public PrqParqueo? ObtenerPorId(int idParqueo) =>
        Cargar().FirstOrDefault(p => p.IdParqueo == idParqueo);

    public List<PrqParqueo> ObtenerTodos() => Cargar().OrderBy(p => p.IdParqueo).ToList();

    public List<PrqParqueo> Buscar(string? provincia = null, string? nombre = null,
                                   decimal? precioMin = null, decimal? precioMax = null) =>
        Cargar().Where(p =>
                Texto.Contiene(p.NombreProvincia, provincia)
                && Texto.Contiene(p.Nombre, nombre)
                && (precioMin is null || p.PrecioPorHora >= precioMin)
                && (precioMax is null || p.PrecioPorHora <= precioMax))
            .OrderBy(p => p.IdParqueo).ToList();
}

public class IngresoJsonRepositorio : IIngresoRepositorio
{
    private static List<PrqIngresoAutomovile> Cargar() =>
        JsonAlmacen.Leer<PrqIngresoAutomovile>("ingresos.json");

    public PrqIngresoAutomovile? ObtenerPorId(int consecutivo) =>
        Cargar().FirstOrDefault(i => i.Consecutivo == consecutivo);

    public List<PrqIngresoAutomovile> ObtenerTodos() => Cargar().OrderBy(i => i.Consecutivo).ToList();

    public decimal? ObtenerPrecioPorHoraPorParqueo(int idParqueo) =>
        JsonAlmacen.Leer<PrqParqueo>("parqueos.json")
            .Where(p => p.IdParqueo == idParqueo)
            .Select(p => (decimal?)p.PrecioPorHora)
            .FirstOrDefault();

    public List<IngresoDetalle> ListarPorTipo(string tipo, DateTime desde, DateTime hasta)
    {
        var autos = JsonAlmacen.Leer<PrqAutomovile>("automoviles.json")
            .Where(a => Texto.Contiene(a.Tipo, tipo)).ToList();
        var parqueos = JsonAlmacen.Leer<PrqParqueo>("parqueos.json");

        return Cargar()
            .Where(i => i.FechaHoraEntrada >= desde && i.FechaHoraEntrada <= hasta)
            .Join(autos, i => i.IdAutomovil, a => a.IdAutomovil, (i, a) => new { i, a })
            .Join(parqueos, x => x.i.IdParqueo, p => p.IdParqueo, (x, p) => new { x.i, x.a, p })
            .OrderBy(f => f.i.FechaHoraEntrada)
            .Select(f => IngresoDetalle.Crear(f.i, f.a, f.p, ObtenerPrecioPorHoraPorParqueo(f.i.IdParqueo)))
            .ToList();
    }

    public List<IngresoDetalle> ListarPorProvincia(string provincia, DateTime desde, DateTime hasta)
    {
        var autos = JsonAlmacen.Leer<PrqAutomovile>("automoviles.json");
        var parqueos = JsonAlmacen.Leer<PrqParqueo>("parqueos.json")
            .Where(p => Texto.Contiene(p.NombreProvincia, provincia)).ToList();

        return Cargar()
            .Where(i => i.FechaHoraEntrada >= desde && i.FechaHoraEntrada <= hasta)
            .Join(autos, i => i.IdAutomovil, a => a.IdAutomovil, (i, a) => new { i, a })
            .Join(parqueos, x => x.i.IdParqueo, p => p.IdParqueo, (x, p) => new { x.i, x.a, p })
            .OrderBy(f => f.i.FechaHoraEntrada)
            .Select(f => IngresoDetalle.Crear(f.i, f.a, f.p, ObtenerPrecioPorHoraPorParqueo(f.i.IdParqueo)))
            .ToList();
    }
}