using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parqueo.Models;
using Parqueo.Repositorios;

namespace Parqueo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParqueosController : ControllerBase
{
    private readonly IParqueoRepositorio _lectura;
    private readonly IEscrituraRepositorio<PrqParqueo> _escritura;

    public ParqueosController(IParqueoRepositorio lectura,
                              IEscrituraRepositorio<PrqParqueo> escritura)
    {
        _lectura = lectura;
        _escritura = escritura;
    }

    // GET api/parqueos
    [HttpGet]
    public ActionResult<List<PrqParqueo>> GetAll() => Ok(_lectura.ObtenerTodos());

    // GET api/parqueos/1  (consulta por llave primaria)
    [HttpGet("{id:int}")]
    public ActionResult<PrqParqueo> GetById(int id)
    {
        var parqueo = _lectura.ObtenerPorId(id);
        return parqueo is null ? NotFound() : Ok(parqueo);
    }

    // GET api/parqueos/buscar?provincia=jos&nombre=central&precioMin=1000&precioMax=2000
    // Provincia y nombre son aproximados; el precio es un rango.
    [HttpGet("buscar")]
    public ActionResult<List<PrqParqueo>> Buscar(
        string? provincia, string? nombre, decimal? precioMin, decimal? precioMax)
        => Ok(_lectura.Buscar(provincia, nombre, precioMin, precioMax));

    // POST api/parqueos
    [HttpPost]
    public ActionResult<PrqParqueo> Post(PrqParqueo parqueo)
    {
        // Se copian solo los campos propios de la tabla
        var nuevo = _escritura.Insertar(new PrqParqueo
        {
            NombreProvincia = parqueo.NombreProvincia,
            Nombre = parqueo.Nombre,
            PrecioPorHora = parqueo.PrecioPorHora
        });
        return CreatedAtAction(nameof(GetById), new { id = nuevo.IdParqueo }, nuevo);
    }

    // PUT api/parqueos/1
    [HttpPut("{id:int}")]
    public IActionResult Put(int id, PrqParqueo parqueo)
    {
        var actualizado = _escritura.Actualizar(new PrqParqueo
        {
            IdParqueo = id,
            NombreProvincia = parqueo.NombreProvincia,
            Nombre = parqueo.Nombre,
            PrecioPorHora = parqueo.PrecioPorHora
        });
        return actualizado ? NoContent() : NotFound();
    }

    // DELETE api/parqueos/1
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            return _escritura.Borrar(id) ? NoContent() : NotFound();
        }
        catch (DbUpdateException)
        {
            // Llave foránea: el parqueo tiene ingresos registrados
            return Conflict("No se puede borrar: el parqueo tiene ingresos registrados.");
        }
    }
}