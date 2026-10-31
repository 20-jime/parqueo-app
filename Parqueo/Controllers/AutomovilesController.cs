using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parqueo.Models;
using Parqueo.Repositorios;

namespace Parqueo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutomovilesController : ControllerBase
{
    private readonly IAutomovilRepositorio _lectura;
    private readonly IEscrituraRepositorio<PrqAutomovile> _escritura;

    public AutomovilesController(IAutomovilRepositorio lectura,
                                 IEscrituraRepositorio<PrqAutomovile> escritura)
    {
        _lectura = lectura;
        _escritura = escritura;
    }

    // GET api/automoviles
    [HttpGet]
    public ActionResult<List<PrqAutomovile>> GetAll() => Ok(_lectura.ObtenerTodos());

    // GET api/automoviles/3  (consulta por llave primaria)
    [HttpGet("{id:int}")]
    public ActionResult<PrqAutomovile> GetById(int id)
    {
        var auto = _lectura.ObtenerPorId(id);
        return auto is null ? NotFound() : Ok(auto);
    }

    // GET api/automoviles/buscar?color=ro&anioDesde=2019&anioHasta=2022&fabricante=toy&tipo=sed
    // Los filtros de texto son aproximados.
    [HttpGet("buscar")]
    public ActionResult<List<PrqAutomovile>> Buscar(
        string? color, int? anioDesde, int? anioHasta, string? fabricante, string? tipo)
        => Ok(_lectura.Buscar(color, anioDesde, anioHasta, fabricante, tipo));

    // POST api/automoviles
    [HttpPost]
    public ActionResult<PrqAutomovile> Post(PrqAutomovile auto)
    {
        // Se copian solo los campos propios de la tabla
        var nuevo = _escritura.Insertar(new PrqAutomovile
        {
            Color = auto.Color,
            Anio = auto.Anio,
            Fabricante = auto.Fabricante,
            Tipo = auto.Tipo
        });
        return CreatedAtAction(nameof(GetById), new { id = nuevo.IdAutomovil }, nuevo);
    }

    // PUT api/automoviles/3
    [HttpPut("{id:int}")]
    public IActionResult Put(int id, PrqAutomovile auto)
    {
        var actualizado = _escritura.Actualizar(new PrqAutomovile
        {
            IdAutomovil = id,
            Color = auto.Color,
            Anio = auto.Anio,
            Fabricante = auto.Fabricante,
            Tipo = auto.Tipo
        });
        return actualizado ? NoContent() : NotFound();
    }

    // DELETE api/automoviles/3
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            return _escritura.Borrar(id) ? NoContent() : NotFound();
        }
        catch (DbUpdateException)
        {
            // Llave foránea: el automóvil tiene ingresos registrados
            return Conflict("No se puede borrar: el automóvil tiene ingresos registrados.");
        }
    }
}