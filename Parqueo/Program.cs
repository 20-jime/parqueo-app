using System.Text.Json.Serialization;
using Parqueo.Data;
using Parqueo.Models;
using Parqueo.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(o =>
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

    builder.Services.AddScoped<IAutomovilRepositorio, AutomovilRepositorio>();
    builder.Services.AddScoped<IEscrituraRepositorio<PrqAutomovile>>(sp =>
        new EscrituraContexto<PrqAutomovile>(
            sp.GetRequiredService<ParqueoContext>(), a => a.IdAutomovil));
}
else
{
    builder.Services.AddSingleton<IAutomovilRepositorio, AutomovilJsonRepositorio>();
    builder.Services.AddSingleton<IEscrituraRepositorio<PrqAutomovile>>(_ =>
        new EscrituraJson<PrqAutomovile>(
            "automoviles.json", a => a.IdAutomovil, (a, id) => a.IdAutomovil = id));
}

// Parqueo e Ingresos se registran en las fases siguientes.

var app = builder.Build();

app.MapControllers();

app.Run();