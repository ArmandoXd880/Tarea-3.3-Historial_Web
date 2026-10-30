using System;
using System.Linq;
using HistorialWeb.Nucleo;

namespace HistorialWeb.Pruebas;

/// <summary>
/// Pruebas automáticas del núcleo. No necesitan abrir la ventana de Windows Forms.
/// Cada caso comprueba una regla concreta del historial de navegación.
/// </summary>
internal static class Program
{
    // Contadores para resumir cuántas verificaciones pasan y cuántas fallan.
    private static int _correctas;
    private static int _fallidas;

    // Ejecuta los doce casos de prueba y presenta el resultado en consola.
    private static void Main()
    {
        // Al comenzar no hay páginas guardadas en ninguna pila.
        Ejecutar("Estado inicial", () =>
        {
            var h = new HistorialNavegacion();
            Verificar(h.PaginaActual == "Inicio" && !h.PuedeVolver && !h.PuedeAvanzar);
        });
        // La última página que abandonamos debe quedar en el tope de Atrás.
        Ejecutar("Tres visitas y orden LIFO", () =>
        {
            var h = CrearTresVisitas();
            Verificar(h.CantidadAtras == 3);
            Verificar(h.PaginasAtras[0] == "https://es.wikipedia.org/");
        });
        // Volver coloca la página abandonada en la pila Adelante.
        Ejecutar("Atrás una vez", () =>
        {
            var h = CrearTresVisitas();
            Verificar(h.Volver());
            Verificar(h.PaginaActual == "https://es.wikipedia.org/");
            Verificar(h.PaginasAdelante[0] == "https://learn.microsoft.com/");
        });
        // Avanzar recupera la página y vuelve a vaciar esa pila.
        Ejecutar("Adelante luego de volver", () =>
        {
            var h = CrearTresVisitas();
            h.Volver();
            Verificar(h.Avanzar() && h.PaginaActual == "https://learn.microsoft.com/");
            Verificar(h.CantidadAdelante == 0);
        });
        // Abrir otra página elimina la ruta que antes permitía avanzar.
        Ejecutar("Visitar otra URL borra adelante", () =>
        {
            var h = CrearTresVisitas();
            h.Volver();
            h.Visitar("github.com");
            Verificar(h.PaginaActual == "https://github.com/");
            Verificar(!h.PuedeAvanzar && !h.Avanzar());
        });
        // Visitar la misma dirección no agrega entradas repetidas.
        Ejecutar("No se duplica la página actual", () =>
        {
            var h = new HistorialNavegacion();
            Verificar(h.Visitar("https://example.com"));
            Verificar(!h.Visitar("https://example.com/"));
            Verificar(h.CantidadAtras == 1);
        });
        // Si no se escribió un protocolo, se utiliza https://.
        Ejecutar("Normaliza URL sin protocolo", () =>
        {
            var h = new HistorialNavegacion();
            h.Visitar("example.com");
            Verificar(h.PaginaActual == "https://example.com/");
        });
        // Una dirección en blanco debe producir un error controlado.
        Ejecutar("Rechaza dirección vacía", () =>
        {
            var h = new HistorialNavegacion();
            Verificar(Rechaza(() => h.Visitar("  ")));
            Verificar(!h.PuedeVolver);
        });
        // FTP no es válido para esta simulación HTTP/HTTPS.
        Ejecutar("Rechaza protocolo no web", () =>
        {
            var h = new HistorialNavegacion();
            Verificar(Rechaza(() => h.Visitar("ftp://example.com")));
        });
        // No se puede extraer de una pila vacía.
        Ejecutar("Límites al volver y avanzar", () =>
        {
            var h = new HistorialNavegacion();
            Verificar(!h.Volver() && !h.Avanzar());
            h.Visitar("example.com");
            Verificar(h.Volver() && !h.Volver());
        });
        // Reiniciar elimina el historial y recupera la página Inicio.
        Ejecutar("Reiniciar vacía ambas pilas", () =>
        {
            var h = CrearTresVisitas();
            h.Volver();
            h.Reiniciar();
            Verificar(h.PaginaActual == "Inicio");
            Verificar(h.CantidadAtras == 0 && h.CantidadAdelante == 0);
        });
        // Cambiar una copia obtenida por la vista no modifica el núcleo.
        Ejecutar("Las vistas son copias", () =>
        {
            var h = CrearTresVisitas();
            string[] copia = h.PaginasAtras.ToArray();
            copia[0] = "alterado";
            Verificar(h.PaginasAtras[0] == "https://es.wikipedia.org/");
        });

        Console.WriteLine($"TOTAL: {_correctas} comprobaciones correctas, {_fallidas} fallidas.");
        Environment.ExitCode = _fallidas == 0 ? 0 : 1;
    }

    // Prepara siempre las mismas tres páginas para repetir las pruebas.
    private static HistorialNavegacion CrearTresVisitas()
    {
        var h = new HistorialNavegacion();
        h.Visitar("google.com");
        h.Visitar("es.wikipedia.org");
        h.Visitar("learn.microsoft.com");
        return h;
    }

    // Devuelve true cuando la acción lanza la excepción esperada.
    private static bool Rechaza(Action accion)
    {
        try { accion(); return false; }
        catch (ArgumentException) { return true; }
    }

    // Ejecuta un caso y reporta su resultado sin detener el resto de las pruebas.
    private static void Ejecutar(string titulo, Action prueba)
    {
        try
        {
            prueba();
            Console.WriteLine("CORRECTO: " + titulo);
        }
        catch (Exception ex)
        {
            _fallidas++;
            Console.WriteLine("ERROR: " + titulo + " | " + ex.Message);
        }
    }

    // Una comprobación fallida genera una excepción que Ejecutar contabiliza.
    private static void Verificar(bool condicion)
    {
        if (!condicion) throw new InvalidOperationException("La condición no se cumplió.");
        _correctas++;
    }
}
