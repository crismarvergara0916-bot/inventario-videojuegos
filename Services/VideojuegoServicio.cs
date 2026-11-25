using InventarioVideojuegos.Data;
using InventarioVideojuegos.Models;

namespace InventarioVideojuegos.Services;

/// <summary>
/// Lógica del programa: valida los datos y delega la persistencia al repositorio.
/// </summary>
public class VideojuegoServicio
{
    public static readonly string[] EstadosValidos =
        { "Pendiente", "Jugando", "Completado", "Abandonado" };

    private readonly IVideojuegoRepositorio _repositorio;

    public VideojuegoServicio(IVideojuegoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public int Agregar(Videojuego v)
    {
        Validar(v);
        return _repositorio.Insertar(v);
    }

    public List<Videojuego> Listar(string filtroTitulo = "") =>
        _repositorio.ObtenerTodos(filtroTitulo.Trim());

    public void Modificar(Videojuego v)
    {
        if (v.Id <= 0)
            throw new ValidacionException("Seleccione un videojuego de la lista para modificarlo.");

        Validar(v);
        if (!_repositorio.Actualizar(v))
            throw new ValidacionException("El videojuego ya no existe en la base de datos.");
    }

    public void Eliminar(int id)
    {
        if (id <= 0)
            throw new ValidacionException("Seleccione un videojuego de la lista para eliminarlo.");

        if (!_repositorio.Eliminar(id))
            throw new ValidacionException("El videojuego ya no existe en la base de datos.");
    }

    private static void Validar(Videojuego v)
    {
        if (string.IsNullOrWhiteSpace(v.Titulo))
            throw new ValidacionException("El título es obligatorio.");
        if (v.Titulo.Length > 100)
            throw new ValidacionException("El título no puede superar los 100 caracteres.");
        if (string.IsNullOrWhiteSpace(v.Plataforma))
            throw new ValidacionException("La plataforma es obligatoria.");
        if (string.IsNullOrWhiteSpace(v.Genero))
            throw new ValidacionException("El género es obligatorio.");

        int anioMaximo = DateTime.Now.Year + 1;
        if (v.Anio < 1970 || v.Anio > anioMaximo)
            throw new ValidacionException($"El año debe estar entre 1970 y {anioMaximo}.");

        if (v.Precio < 0)
            throw new ValidacionException("El precio no puede ser negativo.");

        if (!EstadosValidos.Contains(v.Estado))
            throw new ValidacionException("Seleccione un estado válido.");
    }
}
