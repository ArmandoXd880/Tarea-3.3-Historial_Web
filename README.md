# Tarea 3.3 - Historial de navegación web con pilas

Proyecto en **C# y Windows Forms (.NET 8)**. Simula el historial de un navegador utilizando dos pilas LIFO: una para volver y otra para avanzar. **No descarga ni abre páginas web.**

## Abrir en Windows

1. Instala Visual Studio con la carga de trabajo **Desarrollo de escritorio de .NET** y el SDK de **.NET 8**.
2. Descomprime el ZIP, abre `Historial_Web.sln`.
3. En el Explorador de soluciones, botón derecho sobre `Aplicacion` -> **Establecer como proyecto de inicio**.
4. Presiona **F5** para ejecutar. No hay paquetes NuGet externos ni archivos de diseñador.

O mediante terminal en la carpeta principal:

```powershell
dotnet run --project Aplicacion/Aplicacion.csproj
dotnet run --project Pruebas/Pruebas.csproj
```

## Demostración para el video

1. Pulsa **Cargar demostración**: visita Google, Wikipedia y Microsoft Learn.
2. Pulsa **Atrás**: vuelve a Wikipedia y Microsoft Learn pasa a la pila Adelante.
3. Pulsa **Adelante**: vuelve a Microsoft Learn.
4. Pulsa **Atrás** e introduce `github.com` en Dirección; pulsa **Visitar**. La pila Adelante se vacía.
5. Pulsa **Reiniciar**: las dos pilas quedan vacías.

Si necesitas capturas para el reporte, utiliza **Guardar captura PNG** después de una operación. También puedes guardar el registro TXT. En el video debe aparecer tu cámara web en la esquina inferior derecha de la captura.

## Código y pruebas

**Todos los archivos `.cs` están comentados en español:** se explican las clases,
los métodos principales, las dos pilas (operaciones `Push`, `Pop`, `Clear`),
los controles de Windows Forms y el propósito de los doce casos de prueba.
Los comentarios describen el funcionamiento sin alterar su lógica.


- `Nucleo/HistorialNavegacion.cs`: dos pilas genéricas `Stack<string>`, reglas de navegación y validación de direcciones.
- `Aplicacion/FormularioHistorial.cs`: interfaz Windows Forms.
- `Pruebas/Program.cs`: 12 casos automáticos de consola; se ejecutan con el comando indicado.

**Importante:** El proyecto contiene el código fuente, no un `.exe` ya compilado. Al compilarlo en Windows, el ejecutable se genera en `Aplicacion/bin/Debug/net8.0-windows/HistorialWeb.exe` (o en `Release` si eliges esa configuración). Prueba el programa en tu equipo antes de entregar.

## Publicación

Publica el contenido de la carpeta `Historial_Web` en tu repositorio de GitHub. Agrega la URL pública al reporte cuando se haya creado; no inventes enlaces. Publica el video en tu mismo canal de YouTube y copia su enlace al documento.
