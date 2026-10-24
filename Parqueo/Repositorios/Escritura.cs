using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Parqueo.Data;

namespace Parqueo.Repositorios;

// Operaciones de escritura. La misma interfaz sirve para base de datos y JSON,
// y funciona para las tres tablas (todas tienen llave primaria entera).
public interface IEscrituraRepositorio<T> where T : class
{
    T Insertar(T entidad);       // asigna el id nuevo y devuelve la entidad
    bool Actualizar(T entidad);  // false si el registro no existe
    bool Borrar(int id);         // false si el registro no existe
}

// ---------- Versión para la base de datos ----------
public class EscrituraContexto<T> : IEscrituraRepositorio<T> where T : class
{
    private readonly ParqueoContext _contexto;
    private readonly Func<T, int> _obtenerId;

    public EscrituraContexto(ParqueoContext contexto, Func<T, int> obtenerId)
    {
        _contexto = contexto;
        _obtenerId = obtenerId;
    }

    public T Insertar(T entidad)
    {
        _contexto.Set<T>().Add(entidad);
        _contexto.SaveChanges();
        return entidad;
    }

    public bool Actualizar(T entidad)
    {
        var existente = _contexto.Set<T>().Find(_obtenerId(entidad));
        if (existente is null) return false;
        _contexto.Entry(existente).CurrentValues.SetValues(entidad);
        _contexto.SaveChanges();
        return true;
    }

    public bool Borrar(int id)
    {
        var existente = _contexto.Set<T>().Find(id);
        if (existente is null) return false;
        _contexto.Set<T>().Remove(existente);
        _contexto.SaveChanges();
        return true;
    }
}

// ---------- Versión para los archivos JSON ----------
public class EscrituraJson<T> : IEscrituraRepositorio<T> where T : class
{
    private readonly string _archivo;
    private readonly Func<T, int> _obtenerId;
    private readonly Action<T, int> _asignarId;

    public EscrituraJson(string archivo, Func<T, int> obtenerId, Action<T, int> asignarId)
    {
        _archivo = archivo;
        _obtenerId = obtenerId;
        _asignarId = asignarId;
    }

    public T Insertar(T entidad)
    {
        var lista = JsonEscritor.Leer<T>(_archivo);
        var nuevoId = lista.Count == 0 ? 1 : lista.Max(_obtenerId) + 1;
        _asignarId(entidad, nuevoId);
        lista.Add(entidad);
        JsonEscritor.Guardar(_archivo, lista);
        return entidad;
    }

    public bool Actualizar(T entidad)
    {
        var lista = JsonEscritor.Leer<T>(_archivo);
        var id = _obtenerId(entidad);
        var posicion = lista.FindIndex(x => _obtenerId(x) == id);
        if (posicion < 0) return false;
        lista[posicion] = entidad;
        JsonEscritor.Guardar(_archivo, lista);
        return true;
    }

    public bool Borrar(int id)
    {
        var lista = JsonEscritor.Leer<T>(_archivo);
        if (lista.RemoveAll(x => _obtenerId(x) == id) == 0) return false;
        JsonEscritor.Guardar(_archivo, lista);
        return true;
    }
}

// Lee y escribe los JSON guardando solo las columnas de la tabla
// (se omiten las propiedades de navegación y los campos calculados)
public static class JsonEscritor
{
    private static readonly JsonSerializerOptions Opciones = Crear();

    private static JsonSerializerOptions Crear()
    {
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(info =>
        {
            if (info.Kind != JsonTypeInfoKind.Object) return;
            for (int i = info.Properties.Count - 1; i >= 0; i--)
                if (Excluir(info.Properties[i].Name)) info.Properties.RemoveAt(i);
        });

        return new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // conserva las tildes
            TypeInfoResolver = resolver
        };
    }

    private static bool Excluir(string nombre) =>
        nombre.EndsWith("Navigation") || nombre.StartsWith("Prq")
        || nombre is "DuracionMinutos" or "DuracionHoras" or "MontoTotal" or "PrecioPorHoraAplicado";

    public static List<T> Leer<T>(string archivo)
    {
        var ruta = Path.Combine(JsonAlmacen.CarpetaJson(), archivo);
        return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(ruta), Opciones) ?? new List<T>();
    }

    public static void Guardar<T>(string archivo, List<T> lista)
    {
        var ruta = Path.Combine(JsonAlmacen.CarpetaJson(), archivo);
        File.WriteAllText(ruta, JsonSerializer.Serialize(lista, Opciones));
    }
}