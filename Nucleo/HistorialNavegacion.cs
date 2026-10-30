using System;
using System.Collections.Generic;

namespace HistorialWeb.Nucleo;

/// <summary>
/// Simula el historial de navegación con dos pilas LIFO.
/// No realiza peticiones de Internet ni abre páginas reales.
/// </summary>
public sealed class HistorialNavegacion
{
    // Página inicial antes de visitar cualquier dirección.
    public const string Inicio = "Inicio";
    // Pila para regresar: en el tope siempre está la página anterior.
    private readonly Stack<string> _atras = new();
    // Pila para avanzar: guarda páginas abandonadas al pulsar Atrás.
    private readonly Stack<string> _adelante = new();

    // El formulario puede consultar el estado, pero solo esta clase lo modifica.
    public string PaginaActual { get; private set; } = Inicio;
    // Contadores y condiciones usados para mostrar el estado y habilitar botones.
    public int CantidadAtras => _atras.Count;
    public int CantidadAdelante => _adelante.Count;
    public bool PuedeVolver => _atras.Count > 0;
    public bool PuedeAvanzar => _adelante.Count > 0;

    // Devuelve una copia: el formulario no puede modificar las pilas internas.
    public IReadOnlyList<string> PaginasAtras => _atras.ToArray();
    public IReadOnlyList<string> PaginasAdelante => _adelante.ToArray();

    /// <summary>
    /// Visita una página nueva. Primero conserva la actual en la pila Atrás
    /// y después borra Adelante, como sucede en el historial de un navegador.
    /// </summary>
    /// <returns>True si cambió la página; false si ya estaba en esa URL.</returns>
    public bool Visitar(string entrada)
    {
        // Validamos la entrada antes de modificar el historial.
        string url = NormalizarDireccion(entrada);
        if (string.Equals(url, PaginaActual, StringComparison.OrdinalIgnoreCase))
            return false;

        // Push agrega la página actual al tope de la pila Atrás.
        _atras.Push(PaginaActual);
        PaginaActual = url;
        _adelante.Clear(); // Visitar una página nueva invalida el futuro anterior.
        return true;
    }

    /// <summary>
    /// Retrocede una página. Guarda la actual en Adelante y extrae (Pop)
    /// la última página almacenada en Atrás. Si está vacía, no hace nada.
    /// </summary>
    public bool Volver()
    {
        if (_atras.Count == 0)
            return false;

        // La página que dejamos pasa a estar disponible para avanzar.
        _adelante.Push(PaginaActual);
        PaginaActual = _atras.Pop();
        return true;
    }

    /// <summary>
    /// Hace la operación inversa de Volver utilizando la pila Adelante.
    /// </summary>
    public bool Avanzar()
    {
        if (_adelante.Count == 0)
            return false;

        // Conservamos la actual en Atrás antes de recuperar la siguiente.
        _atras.Push(PaginaActual);
        PaginaActual = _adelante.Pop();
        return true;
    }

    /// <summary>
    /// Limpia las dos pilas y regresa al estado inicial de la simulación.
    /// </summary>
    public void Reiniciar()
    {
        _atras.Clear();
        _adelante.Clear();
        PaginaActual = Inicio;
    }

    /// <summary>
    /// Revisa que la dirección esté escrita y sea HTTP o HTTPS.
    /// Si falta el protocolo, agrega https:// para simplificar la entrada.
    /// </summary>
    private static string NormalizarDireccion(string entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada))
            throw new ArgumentException("Escribe una dirección web.");

        string direccion = entrada.Trim();
        if (!direccion.Contains("://", StringComparison.Ordinal))
            direccion = "https://" + direccion;

        if (!Uri.TryCreate(direccion, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new ArgumentException("Usa una dirección HTTP o HTTPS válida.");
        }

        // AbsoluteUri devuelve un formato consistente para comparar páginas.
        return uri.AbsoluteUri;
    }
}
