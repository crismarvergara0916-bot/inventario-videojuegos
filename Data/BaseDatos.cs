using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Data;

/// <summary>
/// Se encarga de ubicar el archivo SQLite y de crear la tabla si no existe.
/// </summary>
public static class BaseDatos
{
    private static readonly string Carpeta = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "InventarioVideojuegos");

    public static string RutaArchivo => Path.Combine(Carpeta, "inventario.db");

    public static string CadenaConexion => $"Data Source={RutaArchivo}";

    private const string SqlCrearTabla = @"
        CREATE TABLE IF NOT EXISTS Videojuegos (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            Titulo      TEXT    NOT NULL,
            Plataforma  TEXT    NOT NULL,
            Genero      TEXT    NOT NULL,
            Anio        INTEGER NOT NULL,
            Precio      REAL    NOT NULL,
            Estado      TEXT    NOT NULL
        );";

    /// <summary>Crea la carpeta, el archivo .db y la tabla (si no existen).</summary>
    public static void Inicializar()
    {
        Directory.CreateDirectory(Carpeta);

        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlCrearTabla;
        comando.ExecuteNonQuery();
    }
}
