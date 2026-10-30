using HistorialWeb.Nucleo;

namespace HistorialWeb.Aplicacion;

/// <summary>
/// Ventana principal de Windows Forms. Presenta las pilas y conecta
/// los botones con la clase HistorialNavegacion del proyecto Nucleo.
/// </summary>
public sealed class FormularioHistorial : Form
{
    // La lógica del historial está separada de los controles visuales.
    private readonly HistorialNavegacion _historial = new();

    // Controles que muestran la dirección, la página, las dos pilas y el registro.
    private readonly TextBox _direccion = new();

    private readonly Label _paginaActual = new();
    private readonly Label _estado = new();
    private readonly ListBox _listaAtras = new();
    private readonly ListBox _listaAdelante = new();
    private readonly TextBox _registro = new();

    // Estos botones se asignan al crear la interfaz, por eso NO son readonly.
    private Button _volver = new();

    private Button _avanzar = new();

    /// <summary>
    /// Configura el aspecto de la ventana, crea los controles y muestra Inicio.
    /// </summary>
    public FormularioHistorial()
    {
        Text = "Tarea 3.3 | Historial de navegación con pilas";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(930, 675);
        Size = new Size(1120, 780);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(246, 249, 252);
        ConstruirInterfaz();
        ActualizarVista();
        Escribir("Programa iniciado. El historial es una simulación: no abre sitios web.");
    }

    /// <summary>
    /// Crea botones con el mismo formato y les conecta una acción al hacer clic.
    /// </summary>
    private static Button CrearBoton(string texto, EventHandler accion)
    {
        Button boton = new Button
        {
            Text = texto,
            AutoSize = true,
            Height = 36,
            Margin = new Padding(3, 2, 8, 2),
            Padding = new Padding(12, 5, 12, 5),
            FlatStyle = FlatStyle.System
        };
        boton.Click += accion;
        return boton;
    }

    // Genera los títulos que aparecen sobre las distintas secciones.
    private static Label CrearTitulo(string texto) => new()
    {
        Text = texto,
        Dock = DockStyle.Top,
        AutoSize = true,
        Padding = new Padding(0, 0, 0, 7),
        Font = new Font("Segoe UI", 11F, FontStyle.Bold)
    };

    /// <summary>
    /// Construye la interfaz mediante código C#, sin utilizar un archivo Designer.
    /// Distribuye cabecera, dirección, botones, pilas y registro de operaciones.
    /// </summary>
    private void ConstruirInterfaz()
    {
        // Contenedor principal: organiza la ventana en cinco filas.
        TableLayoutPanel principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(23, 17, 23, 18)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 79));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 61));
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 57));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 66));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
        Controls.Add(principal);

        // Primera fila: nombre y propósito de la aplicación.
        Panel cabecera = new Panel { Dock = DockStyle.Fill };
        cabecera.Controls.Add(new Label
        {
            Text = "HISTORIAL DE NAVEGACIÓN WEB",
            Font = new Font("Segoe UI", 19F, FontStyle.Bold),
            ForeColor = Color.FromArgb(28, 60, 105),
            Dock = DockStyle.Top,
            Height = 42
        });
        cabecera.Controls.Add(new Label
        {
            Text = "Estructuras lineales • dos pilas LIFO • simulación sin conexión a Internet",
            ForeColor = Color.FromArgb(75, 86, 102),
            Dock = DockStyle.Bottom,
            Height = 28
        });
        principal.Controls.Add(cabecera, 0, 0);

        // Segunda fila: campo para escribir una dirección y botón Visitar.
        TableLayoutPanel barraDireccion = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
        barraDireccion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        barraDireccion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barraDireccion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
        barraDireccion.Controls.Add(new Label
        {
            Text = "Dirección:",
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill
        }, 0, 0);
        _direccion.Dock = DockStyle.Fill;
        _direccion.Margin = new Padding(0, 4, 11, 13);
        _direccion.PlaceholderText = "Ejemplo: wikipedia.org";
        // Enter tiene el mismo efecto que presionar el botón Visitar.
        _direccion.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                VisitarPagina();
            }
        };
        barraDireccion.Controls.Add(_direccion, 1, 0);
        var visitar = CrearBoton("Visitar", (_, _) => VisitarPagina());
        visitar.Dock = DockStyle.Top;
        barraDireccion.Controls.Add(visitar, 2, 0);
        principal.Controls.Add(barraDireccion, 0, 1);

        // Tercera fila: navegación, demostración y reinicio.
        FlowLayoutPanel acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        _volver = CrearBoton("← Atrás", (_, _) => VolverPagina());
        _avanzar = CrearBoton("Adelante →", (_, _) => AvanzarPagina());
        acciones.Controls.Add(_volver);
        acciones.Controls.Add(_avanzar);
        acciones.Controls.Add(CrearBoton("Cargar demostración", (_, _) => CargarDemostracion()));
        acciones.Controls.Add(CrearBoton("Reiniciar", (_, _) => Reiniciar()));
        principal.Controls.Add(acciones, 0, 2);

        // Cuarta fila: pila Atrás | página actual | pila Adelante.
        TableLayoutPanel panelDatos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
        panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.4F));
        panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
        principal.Controls.Add(panelDatos, 0, 3);

        var izquierda = CrearPanelPila("PILA ATRÁS · tope primero", _listaAtras);
        Panel centro = new Panel { Dock = DockStyle.Fill, Padding = new Padding(13), BackColor = Color.White };
        centro.Controls.Add(new Label
        {
            Dock = DockStyle.Top,
            Height = 44,
            Text = "PÁGINA ACTUAL",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 74, 126)
        });
        _paginaActual.Dock = DockStyle.Fill;
        _paginaActual.TextAlign = ContentAlignment.MiddleCenter;
        _paginaActual.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        _paginaActual.ForeColor = Color.FromArgb(32, 55, 78);
        centro.Controls.Add(_paginaActual);
        _paginaActual.BringToFront();
        // En un Panel, el orden de agregación también determina el orden de acoplamiento.
        TableLayoutPanel actualHolder = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        actualHolder.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        actualHolder.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        actualHolder.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        centro.Controls.Clear();
        actualHolder.Controls.Add(CrearTitulo("PÁGINA ACTUAL"), 0, 0);
        _paginaActual.Dock = DockStyle.Fill;
        actualHolder.Controls.Add(_paginaActual, 0, 1);
        _estado.Dock = DockStyle.Fill;
        _estado.ForeColor = Color.FromArgb(75, 86, 102);
        _estado.TextAlign = ContentAlignment.MiddleCenter;
        actualHolder.Controls.Add(_estado, 0, 2);
        centro.Controls.Add(actualHolder);
        var derecha = CrearPanelPila("PILA ADELANTE · tope primero", _listaAdelante);
        izquierda.Margin = new Padding(0, 0, 10, 0);
        centro.Margin = new Padding(0, 0, 10, 0);
        panelDatos.Controls.Add(izquierda, 0, 0);
        panelDatos.Controls.Add(centro, 1, 0);
        panelDatos.Controls.Add(derecha, 2, 0);

        // Última fila: registro visible y opciones de guardar evidencias.
        TableLayoutPanel inferior = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        inferior.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        inferior.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        principal.Controls.Add(inferior, 0, 4);
        FlowLayoutPanel fila = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        fila.Controls.Add(CrearTitulo("REGISTRO DE OPERACIONES"));
        fila.Controls.Add(CrearBoton("Guardar captura PNG", (_, _) => GuardarCaptura()));
        fila.Controls.Add(CrearBoton("Guardar registro TXT", (_, _) => GuardarRegistro()));
        inferior.Controls.Add(fila, 0, 0);
        _registro.Dock = DockStyle.Fill;
        _registro.Multiline = true;
        _registro.ScrollBars = ScrollBars.Vertical;
        _registro.ReadOnly = true;
        _registro.BackColor = Color.White;
        _registro.Font = new Font("Consolas", 9.5F);
        inferior.Controls.Add(_registro, 0, 1);
    }

    /// <summary>
    /// Crea un panel que muestra los elementos de una pila; el tope va primero.
    /// </summary>
    private static Panel CrearPanelPila(string titulo, ListBox lista)
    {
        Panel panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(11), BackColor = Color.White };
        panel.Controls.Add(lista);
        lista.Dock = DockStyle.Fill;
        lista.Font = new Font("Segoe UI", 9.5F);
        lista.IntegralHeight = false;
        lista.HorizontalScrollbar = true;
        panel.Controls.Add(CrearTitulo(titulo));
        return panel;
    }

    /// <summary>
    /// Atiende el botón Visitar: valida la dirección desde el núcleo, registra
    /// el resultado y actualiza la pantalla. Muestra un aviso si hay un error.
    /// </summary>
    private void VisitarPagina()
    {
        try
        {
            bool cambio = _historial.Visitar(_direccion.Text);
            Escribir(cambio ? "VISITAR: " + _historial.PaginaActual : "Sin cambio: esa página ya está abierta.");
            ActualizarVista();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(this, ex.Message, "Dirección no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Escribir("ERROR: " + ex.Message);
        }
    }

    // Usa la pila Atrás y muestra la nueva página actual.
    private void VolverPagina()
    {
        bool cambio = _historial.Volver();
        Escribir(cambio ? "ATRÁS: " + _historial.PaginaActual : "No hay más páginas hacia atrás.");
        ActualizarVista();
    }

    // Usa la pila Adelante y muestra la nueva página actual.
    private void AvanzarPagina()
    {
        bool cambio = _historial.Avanzar();
        Escribir(cambio ? "ADELANTE: " + _historial.PaginaActual : "No hay más páginas hacia adelante.");
        ActualizarVista();
    }

    private void CargarDemostracion()
    {
        _historial.Reiniciar();
        _registro.Clear();
        Escribir("DEMOSTRACIÓN: historial reiniciado.");
        foreach (string url in new[]
        {
            "https://www.google.com",
            "https://es.wikipedia.org",
            "https://learn.microsoft.com"
        })
        {
            _historial.Visitar(url);
            Escribir("VISITAR: " + _historial.PaginaActual);
        }
        ActualizarVista();
    }

    // Restablece el historial, la entrada del usuario y el registro de eventos.
    private void Reiniciar()
    {
        _historial.Reiniciar();
        _direccion.Clear();
        _registro.Clear();
        Escribir("REINICIAR: ambas pilas vacías y página actual Inicio.");
        ActualizarVista();
    }

    /// <summary>
    /// Copia el estado del núcleo a los controles. Las listas muestran primero
    /// el elemento del tope, es decir, el próximo que saldría con Pop.
    /// </summary>
    private void ActualizarVista()
    {
        _paginaActual.Text = _historial.PaginaActual;
        _estado.Text = $"Atrás: {_historial.CantidadAtras}   |   Adelante: {_historial.CantidadAdelante}";
        _listaAtras.BeginUpdate();
        _listaAtras.Items.Clear();
        foreach (string pagina in _historial.PaginasAtras)
        {
            _listaAtras.Items.Add(pagina);
        }

        _listaAtras.EndUpdate();
        _listaAdelante.BeginUpdate();
        _listaAdelante.Items.Clear();
        foreach (string pagina in _historial.PaginasAdelante)
        {
            _listaAdelante.Items.Add(pagina);
        }

        _listaAdelante.EndUpdate();
        // Deshabilitar botones sin elementos evita operaciones imposibles.
        _volver.Enabled = _historial.PuedeVolver;
        _avanzar.Enabled = _historial.PuedeAvanzar;
    }

    // Añade un mensaje con hora para que cada acción quede visible.
    private void Escribir(string mensaje)
    {
        _registro.AppendText($"{DateTime.Now:HH:mm:ss} | {mensaje}{Environment.NewLine}");
    }

    // Exporta los pasos realizados a un archivo de texto de evidencias.
    private void GuardarRegistro()
    {
        using SaveFileDialog dialogo = new SaveFileDialog
        {
            Filter = "Archivo de texto (*.txt)|*.txt",
            FileName = "Registro_Tarea_3_3.txt"
        };
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            File.WriteAllText(dialogo.FileName, _registro.Text);
            Escribir("Registro guardado en TXT.");
        }
    }

    // Guarda la parte interior del formulario como imagen PNG.
    private void GuardarCaptura()
    {
        using SaveFileDialog dialogo = new SaveFileDialog
        {
            Filter = "Imagen PNG (*.png)|*.png",
            FileName = "Captura_Tarea_3_3.png"
        };
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            using Bitmap imagen = new Bitmap(ClientSize.Width, ClientSize.Height);
            DrawToBitmap(imagen, new Rectangle(Point.Empty, ClientSize));
            imagen.Save(dialogo.FileName, System.Drawing.Imaging.ImageFormat.Png);
            Escribir("Captura guardada en PNG.");
        }
    }
}