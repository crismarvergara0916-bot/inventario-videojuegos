namespace InventarioVideojuegos.Services;

/// <summary>Error de regla de negocio con un mensaje comprensible para el usuario.</summary>
public class ValidacionException : Exception
{
    public ValidacionException(string mensaje) : base(mensaje) { }
}
