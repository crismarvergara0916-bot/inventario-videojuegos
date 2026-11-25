using InventarioVideojuegos.Models;
using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Data;

/// <summary>
/// DAO: aquí se realizan las operaciones CRUD con ADO.NET (Microsoft.Data.Sqlite).
/// Todas las consultas usan parámetros y las conexiones se cierran con 'using'.
/// </summary>
public class VideojuegoDAO : IVideojuegoRepositorio
{
    private readonly string _cadenaConexion;

    public VideojuegoDAO(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    // ---------- CREATE ----------
    public int Insertar(Videojuego v)
    {
        const string sql = @"
            INSERT INTO Videojuegos (Titulo, Plataforma, Genero, Anio, Precio, Estado)
            VALUES (@titulo, @plataforma, @genero, @anio, @precio, @estado);
            SELECT last_insert_rowid();";

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando, v);

        int id = Convert.ToInt32(comando.ExecuteScalar());
        v.AsignarId(id);
        return id;
    }

    // ---------- READ ----------
    public List<Videojuego> ObtenerTodos(string filtroTitulo = "")
    {
        const string sql = @"
            SELECT Id, Titulo, Plataforma, Genero, Anio, Precio, Estado
            FROM Videojuegos
            WHERE Titulo LIKE @filtro
            ORDER BY Titulo;";

        var lista = new List<Videojuego>();

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@filtro", $"%{filtroTitulo}%");

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Videojuego(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetString(2),
                lector.GetString(3),
                lector.GetInt32(4),
                lector.GetDecimal(5),
                lector.GetString(6)));
        }
        return lista;
    }

    // ---------- UPDATE ----------
    public bool Actualizar(Videojuego v)
    {
        const string sql = @"
            UPDATE Videojuegos
            SET Titulo = @titulo, Plataforma = @plataforma, Genero = @genero,
                Anio = @anio, Precio = @precio, Estado = @estado
            WHERE Id = @id;";

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando, v);
        comando.Parameters.AddWithValue("@id", v.Id);

        return comando.ExecuteNonQuery() > 0;
    }

    // ---------- DELETE ----------
    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM Videojuegos WHERE Id = @id;";

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    private static void AgregarParametros(SqliteCommand comando, Videojuego v)
    {
        comando.Parameters.AddWithValue("@titulo", v.Titulo);
        comando.Parameters.AddWithValue("@plataforma", v.Plataforma);
        comando.Parameters.AddWithValue("@genero", v.Genero);
        comando.Parameters.AddWithValue("@anio", v.Anio);
        comando.Parameters.AddWithValue("@precio", v.Precio);
        comando.Parameters.AddWithValue("@estado", v.Estado);
    }
}
