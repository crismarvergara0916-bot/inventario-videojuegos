using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using InventarioVideojuegos.Data;
using InventarioVideojuegos.Models;
using InventarioVideojuegos.Services;

namespace InventarioVideojuegos.Views;

/// <summary>
/// Interfaz gráfica: solo captura datos, llama al servicio y muestra resultados.
/// No contiene SQL ni reglas de negocio.
/// </summary>
public partial class MainWindow : Window
{
    private readonly VideojuegoServicio? _servicio;
    private Videojuego? _seleccionado;
    private bool _cargandoFormulario;

    public MainWindow()
    {
        InitializeComponent();

        CmbEstado.ItemsSource = VideojuegoServicio.EstadosValidos;
        CmbEstado.SelectedIndex = 0;

        try
        {
            BaseDatos.Inicializar();
            _servicio = new VideojuegoServicio(new VideojuegoDAO(BaseDatos.CadenaConexion));
            CargarLista();
            MostrarExito("Base de datos lista.");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo iniciar la base de datos: {ex.Message}");
        }
    }

    // ===================== EVENTOS =====================

    private void BtnAgregar_Click(object? sender, RoutedEventArgs e)
    {
        Ejecutar(() =>
        {
            var v = LeerFormulario(id: 0);
            _servicio!.Agregar(v);
            LimpiarFormulario();
            CargarLista();
            MostrarExito($"Videojuego '{v.Titulo}' agregado correctamente.");
        });
    }

    private void BtnActualizar_Click(object? sender, RoutedEventArgs e)
    {
        Ejecutar(() =>
        {
            if (_seleccionado == null)
                throw new ValidacionException("Seleccione un videojuego de la lista para modificarlo.");

            var v = LeerFormulario(_seleccionado.Id);
            _servicio!.Modificar(v);
            LimpiarFormulario();
            CargarLista();
            MostrarExito($"Videojuego '{v.Titulo}' actualizado correctamente.");
        });
    }

    private async void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_seleccionado == null)
        {
            MostrarError("Seleccione un videojuego de la lista para eliminarlo.");
            return;
        }

        var objetivo = _seleccionado;
        bool confirmado = await ConfirmarAsync($"¿Eliminar '{objetivo.Titulo}'? Esta acción no se puede deshacer.");
        if (!confirmado) return;

        Ejecutar(() =>
        {
            _servicio!.Eliminar(objetivo.Id);
            LimpiarFormulario();
            CargarLista();
            MostrarExito($"Videojuego '{objetivo.Titulo}' eliminado.");
        });
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        MostrarExito("Formulario limpiado.");
    }

    private void TxtBuscar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_servicio == null) return;
        Ejecutar(CargarLista);
    }

    private void GridVideojuegos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_cargandoFormulario) return;

        _seleccionado = GridVideojuegos.SelectedItem as Videojuego;
        if (_seleccionado == null) return;

        TxtTitulo.Text = _seleccionado.Titulo;
        TxtPlataforma.Text = _seleccionado.Plataforma;
        TxtGenero.Text = _seleccionado.Genero;
        TxtAnio.Text = _seleccionado.Anio.ToString(CultureInfo.InvariantCulture);
        TxtPrecio.Text = _seleccionado.Precio.ToString("0.##", CultureInfo.InvariantCulture);
        CmbEstado.SelectedItem = _seleccionado.Estado;
    }

    // ===================== AUXILIARES =====================

    private void CargarLista()
    {
        _cargandoFormulario = true;
        GridVideojuegos.ItemsSource = _servicio!.Listar(TxtBuscar.Text ?? "");
        _cargandoFormulario = false;
    }

    /// <summary>Convierte los textos de la interfaz en un objeto Videojuego.</summary>
    private Videojuego LeerFormulario(int id)
    {
        if (!int.TryParse(TxtAnio.Text?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int anio))
            throw new ValidacionException("El año debe ser un número entero (ej: 2023).");

        string textoPrecio = (TxtPrecio.Text ?? "").Trim().Replace(',', '.');
        if (!decimal.TryParse(textoPrecio, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio))
            throw new ValidacionException("El precio debe ser un número (ej: 59.99).");

        string estado = CmbEstado.SelectedItem as string ?? "";

        return new Videojuego(id, TxtTitulo.Text ?? "", TxtPlataforma.Text ?? "",
                              TxtGenero.Text ?? "", anio, precio, estado);
    }

    private void LimpiarFormulario()
    {
        _seleccionado = null;
        _cargandoFormulario = true;
        GridVideojuegos.SelectedItem = null;
        _cargandoFormulario = false;

        TxtTitulo.Text = "";
        TxtPlataforma.Text = "";
        TxtGenero.Text = "";
        TxtAnio.Text = "";
        TxtPrecio.Text = "";
        CmbEstado.SelectedIndex = 0;
    }

    /// <summary>Ejecuta una acción capturando errores para que el programa nunca se cierre.</summary>
    private void Ejecutar(Action accion)
    {
        if (_servicio == null)
        {
            MostrarError("La base de datos no está disponible.");
            return;
        }

        try
        {
            accion();
        }
        catch (ValidacionException ex)
        {
            MostrarError(ex.Message);
        }
        catch (Exception ex)
        {
            MostrarError($"Error inesperado: {ex.Message}");
        }
    }

    private void MostrarExito(string mensaje)
    {
        TxtMensaje.Foreground = Brushes.ForestGreen;
        TxtMensaje.Text = "✔ " + mensaje;
    }

    private void MostrarError(string mensaje)
    {
        TxtMensaje.Foreground = Brushes.Firebrick;
        TxtMensaje.Text = "✖ " + mensaje;
    }

    private async Task<bool> ConfirmarAsync(string mensaje)
    {
        bool resultado = false;
        var dialogo = new Window
        {
            Title = "Confirmar",
            Width = 380,
            Height = 170,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var btnSi = new Button { Content = "Sí" };
        var btnNo = new Button { Content = "No" };
        btnSi.Click += (_, _) => { resultado = true; dialogo.Close(); };
        btnNo.Click += (_, _) => dialogo.Close();

        dialogo.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(16),
            Spacing = 16,
            Children =
            {
                new TextBlock { Text = mensaje, TextWrapping = TextWrapping.Wrap },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Children = { btnSi, btnNo }
                }
            }
        };

        await dialogo.ShowDialog(this);
        return resultado;
    }
}
