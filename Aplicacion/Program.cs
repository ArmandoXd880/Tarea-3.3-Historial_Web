using System;
using System.Windows.Forms;

namespace HistorialWeb.Aplicacion;

/// <summary>
/// Punto de entrada de la aplicación gráfica de Windows Forms.
/// </summary>
internal static class Program
{
    // STAThread es el modelo de subprocesos requerido por componentes gráficos
    // de Windows, incluidos algunos cuadros de diálogo del formulario.
    [STAThread]
    private static void Main()
    {
        // Prepara la configuración visual y después abre la ventana principal.
        ApplicationConfiguration.Initialize();
        Application.Run(new FormularioHistorial());
    }
}
