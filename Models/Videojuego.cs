namespace InventarioVideojuegos.Models;

/// <summary>
/// Entidad principal del sistema. Representa un videojuego del inventario.
/// Aplica encapsulamiento: campos privados, propiedades controladas y Id con set privado.
/// </summary>
public class Videojuego
{
    private string _titulo = "";
    private string _plataforma = "";
    private string _genero = "";
    private string _estado = "";

    public int Id { get; private set; }

    public string Titulo
    {
        get => _titulo;
        set => _titulo = (value ?? "").Trim();
    }

    public string Plataforma
    {
        get => _plataforma;
        set => _plataforma = (value ?? "").Trim();
    }

    public string Genero
    {
        get => _genero;
        set => _genero = (value ?? "").Trim();
    }

    public int Anio { get; set; }
    public decimal Precio { get; set; }

    public string Estado
    {
        get => _estado;
        set => _estado = (value ?? "").Trim();
    }

    /// <summary>Constructor para un videojuego nuevo (todavía sin Id).</summary>
    public Videojuego(string titulo, string plataforma, string genero,
                      int anio, decimal precio, string estado)
    {
        Titulo = titulo;
        Plataforma = plataforma;
        Genero = genero;
        Anio = anio;
        Precio = precio;
        Estado = estado;
    }

    /// <summary>Constructor para un videojuego leído desde la base de datos.</summary>
    public Videojuego(int id, string titulo, string plataforma, string genero,
                      int anio, decimal precio, string estado)
        : this(titulo, plataforma, genero, anio, precio, estado)
    {
        Id = id;
    }

    /// <summary>Permite al DAO asignar el Id generado por SQLite tras un INSERT.</summary>
    public void AsignarId(int id) => Id = id;

    public override string ToString() => $"{Titulo} ({Plataforma}, {Anio})";
}
