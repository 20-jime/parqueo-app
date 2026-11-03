using Microsoft.AspNetCore.Mvc;
 
namespace Parqueo.Controllers;
 
// Controlador MVC (vistas). No accede a datos: las vistas llaman con JavaScript
// (fetch) a los métodos de api/automoviles, que usan los repositorios.
public class VehiculosController : Controller
{
    // /Vehiculos  -> filtro por intervalo de años y listado con botones
    public IActionResult Index() => View();
 
    // /Vehiculos/Detalles/3
    public IActionResult Detalles(int id) => View(id);
 
    // /Vehiculos/Crear
    public IActionResult Crear() => View("Formulario", 0);
 
    // /Vehiculos/Editar/3
    public IActionResult Editar(int id) => View("Formulario", id);
}