using Microsoft.AspNetCore.Mvc;
using Parqueo.Models;
using Parqueo.Repositorios;

namespace Parqueo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngresosController : ControllerBase
{
    private readonly IIngresoRepositorio _lectura;
    private readonly IEscrituraRepositorio<PrqIngresoAutomovile> _escritura;
    private readonly IAutomovilRepositorio _autos;
    private readonly IParqueoRepositorio _parqueos;

    public IngresosController(
        IIngresoRepositorio lectura,
        IEscrituraRepositorio<PrqIngresoAutomovile> escritura,
        IAutomovilRepositorio autos,
        IParqueoRepositorio parqueos)
    {
        _lectura = lectura;
        _escritura = escritura;
        _autos = autos;
        _parqueos = parqueos;
    }

    // Llena las propiedades de navegación y el precio por hora,
    // para que MontoTotal, DuracionMinutos y DuracionHoras salgan calculados.
    private PrqIngresoAutomovile Completar(PrqIngresoAutomovile ingreso)
    {
        ingreso.IdAutomovilNavigation = _autos.ObtenerPorId(ingreso.IdAutomovil)!;
        ingreso.IdParqueoNavigation = _parqueos.ObtenerPorId(ingreso.IdParqueo)!;
        ingreso.PrecioPorHoraAplicado = _lectura.ObtenerPrecioPorHoraPorParqueo(ingreso.IdParqueo);
        return ingreso;
    }

    // La fecha "hasta" incluye todo ese día
    private static DateTime FinDelDia(DateTime fecha) => fecha.Date.AddDays(1).AddSeconds(-1);

    // GET api/ingresos
    [HttpGet]
    public ActionResult<List<PrqIngresoAutomovile>> GetAll()
        => Ok(_lectura.ObtenerTodos().Select(Completar).ToList());

    // GET api/ingresos/1  (consulta por llave primaria)
    [HttpGet("{id:int}")]
    public ActionResult<PrqIngresoAutomovile> GetById(int id)
    {
        var ingreso = _lectura.ObtenerPorId(id);
        return ingreso is null ? NotFound() : Ok(Completar(ingreso));
    }

    // GET api/ingresos/por-tipo?tipo=sedan&desde=2026-10-01&hasta=2026-10-09
    [HttpGet("por-tipo")]
    public ActionResult<List<IngresoDetalle>> PorTipo(string? tipo, DateTime? desde, DateTime? hasta)
    {
        if (string.IsNullOrWhiteSpace(tipo) || desde is null || hasta is null)
            return BadRequest("Indique tipo, desde y hasta (por ejemplo desde=2026-10-01).");

        return Ok(_lectura.ListarPorTipo(tipo, desde.Value, FinDelDia(hasta.Value)));
    }

    // GET api/ingresos/por-provincia?provincia=heredia&desde=2026-10-01&hasta=2026-10-31
    [HttpGet("por-provincia")]
    public ActionResult<List<IngresoDetalle>> PorProvincia(string? provincia, DateTime? desde, DateTime? hasta)
    {
        if (string.IsNullOrWhiteSpace(provincia) || desde is null || hasta is null)
            return BadRequest("Indique provincia, desde y hasta (por ejemplo desde=2026-10-01).");

        return Ok(_lectura.ListarPorProvincia(provincia, desde.Value, FinDelDia(hasta.Value)));
    }
        // GET api/ingresos/por-automovil/3
    // Ingresos de un automóvil, con estadía en minutos y horas y monto a cancelar
    [HttpGet("por-automovil/{idAutomovil:int}")]
    public IActionResult PorAutomovil(int idAutomovil)
    {
        if (_autos.ObtenerPorId(idAutomovil) is null) return NotFound();

        var ingresos = _lectura.ObtenerTodos()
            .Where(i => i.IdAutomovil == idAutomovil)
            .OrderBy(i => i.FechaHoraEntrada)
            .Select(i =>
            {
                Completar(i); // asigna el precio por hora para calcular el monto
                return new
                {
                    i.Consecutivo,
                    i.IdParqueo,
                    i.FechaHoraEntrada,
                    i.FechaHoraSalida,
                    i.DuracionMinutos,
                    i.DuracionHoras,
                    i.MontoTotal
                };
            })
            .ToList();

        return Ok(ingresos);
    }

    // POST api/ingresos
    [HttpPost]
    public ActionResult<PrqIngresoAutomovile> Post(PrqIngresoAutomovile ingreso)
    {
        var error = Validar(ingreso);
        if (error is not null) return BadRequest(error);

        // Se copian solo los campos propios de la tabla
        var nuevo = _escritura.Insertar(new PrqIngresoAutomovile
        {
            IdParqueo = ingreso.IdParqueo,
            IdAutomovil = ingreso.IdAutomovil,
            FechaHoraEntrada = ingreso.FechaHoraEntrada,
            FechaHoraSalida = ingreso.FechaHoraSalida
        });

        // Se vuelve a leer para devolverlo completo
        var creado = _lectura.ObtenerPorId(nuevo.Consecutivo)!;
        return CreatedAtAction(nameof(GetById), new { id = creado.Consecutivo }, Completar(creado));
    }

    // PUT api/ingresos/1
    [HttpPut("{id:int}")]
    public IActionResult Put(int id, PrqIngresoAutomovile ingreso)
    {
        var error = Validar(ingreso);
        if (error is not null) return BadRequest(error);

        var actualizado = _escritura.Actualizar(new PrqIngresoAutomovile
        {
            Consecutivo = id,
            IdParqueo = ingreso.IdParqueo,
            IdAutomovil = ingreso.IdAutomovil,
            FechaHoraEntrada = ingreso.FechaHoraEntrada,
            FechaHoraSalida = ingreso.FechaHoraSalida
        });
        return actualizado ? NoContent() : NotFound();
    }

    // DELETE api/ingresos/1
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        => _escritura.Borrar(id) ? NoContent() : NotFound();

    private string? Validar(PrqIngresoAutomovile ingreso)
    {
        if (_parqueos.ObtenerPorId(ingreso.IdParqueo) is null)
            return $"No existe el parqueo {ingreso.IdParqueo}.";
        if (_autos.ObtenerPorId(ingreso.IdAutomovil) is null)
            return $"No existe el automóvil {ingreso.IdAutomovil}.";
        if (ingreso.FechaHoraSalida.HasValue && ingreso.FechaHoraSalida < ingreso.FechaHoraEntrada)
            return "La hora de salida no puede ser anterior a la hora de entrada.";
        return null;
    }
}