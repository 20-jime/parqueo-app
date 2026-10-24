using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Parqueo.Data;

public static class ParqueoContextFactory
{
    // Crea el contexto leyendo la cadena de conexión desde el secreto de usuario
    public static ParqueoContext Crear()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(typeof(ParqueoContextFactory).Assembly)
            .Build();

        var cadena = config.GetConnectionString("Parqueo")
            ?? throw new InvalidOperationException(
                "No se encontró el secreto ConnectionStrings:Parqueo.");

        var opciones = new DbContextOptionsBuilder<ParqueoContext>()
            .UseNpgsql(cadena)
            .Options;

        return new ParqueoContext(opciones);
    }
}