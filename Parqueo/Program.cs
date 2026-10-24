using Parqueo.Data;
using Parqueo.Models;
using Parqueo.Repositorios;

using var contexto = ParqueoContextFactory.Crear();

// ---------- Consultas: mismo código, dos orígenes de datos ----------
Probar("BASE DE DATOS",
    new AutomovilRepositorio(contexto),
    new ParqueoRepositorio(contexto),
    new IngresoRepositorio(contexto));

Probar("JSON",
    new AutomovilJsonRepositorio(),
    new ParqueoJsonRepositorio(),
    new IngresoJsonRepositorio());

// ---------- Insertar, actualizar y borrar ----------
ProbarEscritura("BASE DE DATOS",
    new EscrituraContexto<PrqAutomovile>(contexto, a => a.IdAutomovil),
    new EscrituraContexto<PrqIngresoAutomovile>(contexto, i => i.Consecutivo),
    new AutomovilRepositorio(contexto),
    new IngresoRepositorio(contexto));

ProbarEscritura("JSON",
    new EscrituraJson<PrqAutomovile>("automoviles.json", a => a.IdAutomovil, (a, id) => a.IdAutomovil = id),
    new EscrituraJson<PrqIngresoAutomovile>("ingresos.json", i => i.Consecutivo, (i, id) => i.Consecutivo = id),
    new AutomovilJsonRepositorio(),
    new IngresoJsonRepositorio());

static void Probar(string origen, IAutomovilRepositorio autos,
                   IParqueoRepositorio parqueos, IIngresoRepositorio ingresos)
{
    Console.WriteLine($"\n===== {origen} =====");
    Console.WriteLine($"Automóviles: {autos.ObtenerTodos().Count}");
    Console.WriteLine($"Parqueos: {parqueos.ObtenerTodos().Count}");
    Console.WriteLine($"Ingresos: {ingresos.ObtenerTodos().Count}");

    Console.WriteLine($"Sedanes 2019-2022: {autos.Buscar(tipo: "sed", anioDesde: 2019, anioHasta: 2022).Count}");
    Console.WriteLine($"Parqueos de Heredia hasta 1300: {parqueos.Buscar(provincia: "hered", precioMax: 1300).Count}");
    Console.WriteLine($"Precio por hora del parqueo 1: {ingresos.ObtenerPrecioPorHoraPorParqueo(1)}");

    var desde = new DateTime(2026, 10, 1);
    var hasta = new DateTime(2026, 10, 31, 23, 59, 59);

    Console.WriteLine("Ingresos de vehículos 4x4 en octubre:");
    foreach (var d in ingresos.ListarPorTipo("4x4", desde, hasta))
        Console.WriteLine($"  #{d.Consecutivo} {d.Fabricante} {d.Color} | entrada {d.FechaHoraEntrada:g} | " +
                          $"salida {d.FechaHoraSalida?.ToString("g") ?? "sin salida"} | " +
                          $"monto {d.MontoTotal?.ToString("N2") ?? "N/A"}");

    Console.WriteLine($"Ingresos en parqueos de Heredia en octubre: {ingresos.ListarPorProvincia("heredia", desde, hasta).Count}");
}

static void ProbarEscritura(string origen,
    IEscrituraRepositorio<PrqAutomovile> escrituraAutos,
    IEscrituraRepositorio<PrqIngresoAutomovile> escrituraIngresos,
    IAutomovilRepositorio lecturaAutos,
    IIngresoRepositorio lecturaIngresos)
{
    Console.WriteLine($"\n===== ESCRITURA: {origen} =====");

    var auto = escrituraAutos.Insertar(new PrqAutomovile
        { Color = "Verde", Anio = 2023, Fabricante = "Kia", Tipo = "Sedan" });
    Console.WriteLine($"Insertado automóvil con id {auto.IdAutomovil}");

    auto.Color = "Amarillo";
    escrituraAutos.Actualizar(auto);
    Console.WriteLine($"Color después de actualizar: {lecturaAutos.ObtenerPorId(auto.IdAutomovil)?.Color}");

    var ingreso = escrituraIngresos.Insertar(new PrqIngresoAutomovile
        { IdParqueo = 1, IdAutomovil = auto.IdAutomovil, FechaHoraEntrada = new DateTime(2026, 10, 23, 9, 0, 0) });
    var sinSalida = lecturaIngresos.ObtenerPorId(ingreso.Consecutivo);
    Console.WriteLine($"Ingreso #{ingreso.Consecutivo} sin salida, duración en horas: {sinSalida?.DuracionHoras?.ToString() ?? "nula"}");

    ingreso.FechaHoraSalida = new DateTime(2026, 10, 23, 12, 0, 0);
    escrituraIngresos.Actualizar(ingreso);
    var conSalida = lecturaIngresos.ObtenerPorId(ingreso.Consecutivo);
    Console.WriteLine($"Con salida, duración en horas: {conSalida?.DuracionHoras}");

    // Se limpia lo creado: primero el ingreso (llave foránea) y luego el automóvil
    Console.WriteLine($"Ingreso borrado: {escrituraIngresos.Borrar(ingreso.Consecutivo)}");
    Console.WriteLine($"Automóvil borrado: {escrituraAutos.Borrar(auto.IdAutomovil)}");
    Console.WriteLine($"¿Sigue existiendo el automóvil? {lecturaAutos.ObtenerPorId(auto.IdAutomovil) is not null}");
}