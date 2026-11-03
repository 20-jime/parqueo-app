using System.Text.Json.Serialization;
using Parqueo.Data;
using Parqueo.Models;
using Parqueo.Repositorios;
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllersWithViews(o =>   // CAMBIO 1: antes AddControllers; ahora también sirve vistas MVC
    {
        // Las propiedades de navegación no son obligatorias en POST/PUT
        o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(o =>
    {
        // Evita errores por referencias circulares en las propiedades de navegación
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Fuente de datos: "Json" o "Db" (se cambia en appsettings.json)
var usarBaseDatos = string.Equals(
    builder.Configuration["DataSource"], "Db", StringComparison.OrdinalIgnoreCase);
if (usarBaseDatos)
{
    // Un contexto por petición, creado con tu fábrica (lee el secreto de usuario)
    builder.Services.AddScoped(_ => ParqueoContextFactory.Crear());
    // Automóviles
    builder.Services.AddScoped<IAutomovilRepositorio, AutomovilRepositorio>();
    builder.Services.AddScoped<IEscrituraRepositorio<PrqAutomovile>>(sp =>
        new EscrituraContexto<PrqAutomovile>(
            sp.GetRequiredService<ParqueoContext>(), a => a.IdAutomovil));
    // Parqueos
    builder.Services.AddScoped<IParqueoRepositorio, ParqueoRepositorio>();
    builder.Services.AddScoped<IEscrituraRepositorio<PrqParqueo>>(sp =>
        new EscrituraContexto<PrqParqueo>(
            sp.GetRequiredService<ParqueoContext>(), p => p.IdParqueo));
    // Ingresos
    builder.Services.AddScoped<IIngresoRepositorio, IngresoRepositorio>();
    builder.Services.AddScoped<IEscrituraRepositorio<PrqIngresoAutomovile>>(sp =>
        new EscrituraContexto<PrqIngresoAutomovile>(
            sp.GetRequiredService<ParqueoContext>(), i => i.Consecutivo));
}
else
{
    // Automóviles
    builder.Services.AddSingleton<IAutomovilRepositorio, AutomovilJsonRepositorio>();
    builder.Services.AddSingleton<IEscrituraRepositorio<PrqAutomovile>>(_ =>
        new EscrituraJson<PrqAutomovile>(
            "automoviles.json", a => a.IdAutomovil, (a, id) => a.IdAutomovil = id));
    // Parqueos
    builder.Services.AddSingleton<IParqueoRepositorio, ParqueoJsonRepositorio>();
    builder.Services.AddSingleton<IEscrituraRepositorio<PrqParqueo>>(_ =>
        new EscrituraJson<PrqParqueo>(
            "parqueos.json", p => p.IdParqueo, (p, id) => p.IdParqueo = id));
    // Ingresos
    builder.Services.AddSingleton<IIngresoRepositorio, IngresoJsonRepositorio>();
    builder.Services.AddSingleton<IEscrituraRepositorio<PrqIngresoAutomovile>>(_ =>
        new EscrituraJson<PrqIngresoAutomovile>(
            "ingresos.json", i => i.Consecutivo, (i, id) => i.Consecutivo = id));
}
var app = builder.Build();
app.UseStaticFiles();   // CAMBIO 2
app.MapControllers();   // rutas api/... (atributos)
app.MapControllerRoute( // CAMBIO 3: rutas MVC; la página inicial es /Vehiculos
    name: "default",
    pattern: "{controller=Vehiculos}/{action=Index}/{id?}");
app.Run();