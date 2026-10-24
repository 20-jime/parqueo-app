using Microsoft.EntityFrameworkCore;
using Parqueo.Data;
using Parqueo.Models;

namespace Parqueo.Repositorios;

public class AutomovilRepositorio : IAutomovilRepositorio
{
    private readonly ParqueoContext _contexto;

    public AutomovilRepositorio(ParqueoContext contexto) => _contexto = contexto;

    public PrqAutomovile? ObtenerPorId(int idAutomovil) =>
        _contexto.PrqAutomoviles.AsNoTracking()
            .FirstOrDefault(a => a.IdAutomovil == idAutomovil);

    public List<PrqAutomovile> ObtenerTodos() =>
        _contexto.PrqAutomoviles.AsNoTracking().OrderBy(a => a.IdAutomovil).ToList();

    // Los filtros de texto son aproximados (contienen el texto, sin importar mayúsculas)
    public List<PrqAutomovile> Buscar(string? color = null, int? anioDesde = null, int? anioHasta = null,
                                      string? fabricante = null, string? tipo = null)
    {
        var consulta = _contexto.PrqAutomoviles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(color))
            consulta = consulta.Where(a => EF.Functions.ILike(a.Color, $"%{color}%"));
        if (anioDesde.HasValue)
            consulta = consulta.Where(a => a.Anio >= anioDesde.Value);
        if (anioHasta.HasValue)
            consulta = consulta.Where(a => a.Anio <= anioHasta.Value);
        if (!string.IsNullOrWhiteSpace(fabricante))
            consulta = consulta.Where(a => EF.Functions.ILike(a.Fabricante, $"%{fabricante}%"));
        if (!string.IsNullOrWhiteSpace(tipo))
            consulta = consulta.Where(a => EF.Functions.ILike(a.Tipo, $"%{tipo}%"));

        return consulta.OrderBy(a => a.IdAutomovil).ToList();
    }
}

public class ParqueoRepositorio : IParqueoRepositorio
{
    private readonly ParqueoContext _contexto;

    public ParqueoRepositorio(ParqueoContext contexto) => _contexto = contexto;

    public PrqParqueo? ObtenerPorId(int idParqueo) =>
        _contexto.PrqParqueos.AsNoTracking()
            .FirstOrDefault(p => p.IdParqueo == idParqueo);

    public List<PrqParqueo> ObtenerTodos() =>
        _contexto.PrqParqueos.AsNoTracking().OrderBy(p => p.IdParqueo).ToList();

    public List<PrqParqueo> Buscar(string? provincia = null, string? nombre = null,
                                   decimal? precioMin = null, decimal? precioMax = null)
    {
        var consulta = _contexto.PrqParqueos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(provincia))
            consulta = consulta.Where(p => EF.Functions.ILike(p.NombreProvincia, $"%{provincia}%"));
        if (!string.IsNullOrWhiteSpace(nombre))
            consulta = consulta.Where(p => EF.Functions.ILike(p.Nombre, $"%{nombre}%"));
        if (precioMin.HasValue)
            consulta = consulta.Where(p => p.PrecioPorHora >= precioMin.Value);
        if (precioMax.HasValue)
            consulta = consulta.Where(p => p.PrecioPorHora <= precioMax.Value);

        return consulta.OrderBy(p => p.IdParqueo).ToList();
    }
}

public class IngresoRepositorio : IIngresoRepositorio
{
    private readonly ParqueoContext _contexto;

    public IngresoRepositorio(ParqueoContext contexto) => _contexto = contexto;

    public PrqIngresoAutomovile? ObtenerPorId(int consecutivo) =>
        _contexto.PrqIngresoAutomoviles.AsNoTracking()
            .FirstOrDefault(i => i.Consecutivo == consecutivo);

    public List<PrqIngresoAutomovile> ObtenerTodos() =>
        _contexto.PrqIngresoAutomoviles.AsNoTracking().OrderBy(i => i.Consecutivo).ToList();

    // Devuelve el monto por hora del parqueo (nulo si el parqueo no existe)
    public decimal? ObtenerPrecioPorHoraPorParqueo(int idParqueo) =>
        _contexto.PrqParqueos.AsNoTracking()
            .Where(p => p.IdParqueo == idParqueo)
            .Select(p => (decimal?)p.PrecioPorHora)
            .FirstOrDefault();

    public List<IngresoDetalle> ListarPorTipo(string tipo, DateTime desde, DateTime hasta)
    {
        var filas = (from i in _contexto.PrqIngresoAutomoviles.AsNoTracking()
                     join a in _contexto.PrqAutomoviles.AsNoTracking() on i.IdAutomovil equals a.IdAutomovil
                     join p in _contexto.PrqParqueos.AsNoTracking() on i.IdParqueo equals p.IdParqueo
                     where i.FechaHoraEntrada >= desde && i.FechaHoraEntrada <= hasta
                           && EF.Functions.ILike(a.Tipo, $"%{tipo}%")
                     orderby i.FechaHoraEntrada
                     select new { i, a, p }).ToList();

        return filas.Select(f => IngresoDetalle.Crear(
            f.i, f.a, f.p, ObtenerPrecioPorHoraPorParqueo(f.i.IdParqueo))).ToList();
    }

    public List<IngresoDetalle> ListarPorProvincia(string provincia, DateTime desde, DateTime hasta)
    {
        var filas = (from i in _contexto.PrqIngresoAutomoviles.AsNoTracking()
                     join a in _contexto.PrqAutomoviles.AsNoTracking() on i.IdAutomovil equals a.IdAutomovil
                     join p in _contexto.PrqParqueos.AsNoTracking() on i.IdParqueo equals p.IdParqueo
                     where i.FechaHoraEntrada >= desde && i.FechaHoraEntrada <= hasta
                           && EF.Functions.ILike(p.NombreProvincia, $"%{provincia}%")
                     orderby i.FechaHoraEntrada
                     select new { i, a, p }).ToList();

        return filas.Select(f => IngresoDetalle.Crear(
            f.i, f.a, f.p, ObtenerPrecioPorHoraPorParqueo(f.i.IdParqueo))).ToList();
    }
}