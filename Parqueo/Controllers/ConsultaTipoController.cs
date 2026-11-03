using Microsoft.AspNetCore.Mvc;

namespace Parqueo.Controllers;

// Controlador MVC maestro/detalle. Solo devuelve la vista; los datos se piden
// con fetch a api/automoviles/buscar y api/ingresos/por-automovil/{id}.
public class ConsultaTipoController : Controller
{
    // /ConsultaTipo
    public IActionResult Index() => View();
}
