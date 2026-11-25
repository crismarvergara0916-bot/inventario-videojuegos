using InventarioVideojuegos.Models;

namespace InventarioVideojuegos.Data;

/// <summary>
/// Abstracción del acceso a datos. El servicio depende de esta interfaz
/// y no de SQLite directamente (bajo acoplamiento).
/// </summary>
public interface IVideojuegoRepositorio
{
    int Insertar(Videojuego videojuego);
    List<Videojuego> ObtenerTodos(string filtroTitulo = "");
    bool Actualizar(Videojuego videojuego);
    bool Eliminar(int id);
}
