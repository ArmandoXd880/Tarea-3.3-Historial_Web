# Tarea 3.3 · Historial de navegación web con pilas

**Alumno:** Jesús Armando Zendejo Altamirano  
**Materia:** Estructuras de Datos · Unidad 3  
**Docente:** Ruben Riojas

Aplicación desarrollada en **C# con Windows Forms y .NET 8** para resolver un caso práctico de estructuras lineales: la administración del historial de navegación de un navegador. La solución emplea dos pilas genéricas `Stack<string>` con comportamiento **LIFO** (último en entrar, primero en salir), una para regresar a páginas anteriores y otra para avanzar a páginas visitadas después de retroceder.

El programa **simula la navegación**: administra direcciones y movimientos en el historial, pero no descarga contenido ni muestra páginas web reales. El código fuente está comentado en español y separa la lógica de las estructuras de datos de la interfaz gráfica.

## Problema y justificación de la estructura

Al recorrer varias páginas, un navegador necesita recordar la página anterior para poder regresar. Si el usuario retrocede, también debe conservar la página que dejó para permitirle avanzar. Además, cuando visita una dirección diferente después de retroceder, el historial disponible hacia adelante debe descartarse.

Una **pila** representa adecuadamente estos movimientos porque siempre se recupera primero la página guardada más recientemente. La operación `Push` incorpora un elemento al tope, `Pop` retira el elemento superior y `Clear` vacía la pila. Se utilizan **dos pilas** para administrar de manera independiente los recorridos hacia atrás y hacia adelante. No es necesario acceder por índice a elementos intermedios.

## Abrir y ejecutar

**Requisitos:** Windows, SDK de .NET 8 y Visual Studio con la carga de trabajo **Desarrollo de escritorio de .NET**.

Abrir `Historial_Web.sln`, seleccionar `Aplicacion` como proyecto de inicio y ejecutar con **F5**. La ventana se construye directamente en `FormularioHistorial.cs`, sin archivos de diseñador ni paquetes NuGet externos.

Desde una terminal situada en la carpeta de la solución:

```powershell
dotnet run --project Aplicacion/Aplicacion.csproj -c Release
```

También se incluye `Ejecutar_en_Windows.bat`, que inicia el proyecto gráfico desde Windows si está instalado el SDK correspondiente.

## Componentes y funcionamiento

La ventana muestra tres áreas principales: **Pila Atrás**, **Página actual** y **Pila Adelante**. Las listas visuales presentan el **tope de cada pila en la primera fila**, es decir, la página que se recuperará en el siguiente movimiento. Debajo se indican los contadores y se conserva un registro con la hora y las acciones realizadas.

| Acción | Comportamiento | Operaciones sobre las pilas |
| --- | --- | --- |
| **Visitar** | Valida la dirección, guarda la página actual en Atrás y abre la nueva página simulada. | `Push` en Atrás; `Clear` en Adelante. |
| **Atrás** | Regresa a la página anterior, conservando la actual para poder avanzar. | `Push` en Adelante; `Pop` en Atrás. |
| **Adelante** | Recupera la página a la que se podía avanzar, conservando la actual para volver. | `Push` en Atrás; `Pop` en Adelante. |
| **Reiniciar** | Restablece la página `Inicio` y elimina el historial previo. | `Clear` en ambas pilas. |
| **Cargar demostración** | Reinicia el estado y visita tres direcciones de ejemplo para observar el orden LIFO. | Llamadas consecutivas a `Visitar`. |
| **Guardar captura PNG** | Guarda una imagen del contenido del formulario. | No modifica las pilas. |
| **Guardar registro TXT** | Exporta a texto el registro de operaciones. | No modifica las pilas. |

Los botones **Atrás** y **Adelante** se deshabilitan cuando su pila correspondiente está vacía. La entrada admite direcciones HTTP o HTTPS; si se escribe un dominio sin protocolo, el programa antepone `https://`. Las entradas vacías o con protocolos no admitidos producen un aviso. Visitar exactamente la página actual (tras normalizar su dirección) no crea una nueva entrada en el historial.

## Ejemplo de navegación

La demostración integrada visita, en orden, Google, Wikipedia y Microsoft Learn. La página inicial `Inicio` también se guarda en Atrás al realizar la primera visita. En la tabla, los elementos de cada pila aparecen **del tope hacia el fondo**.

| Paso | Página actual | Pila Atrás | Pila Adelante |
| --- | --- | --- | --- |
| Estado inicial | `Inicio` | Vacía | Vacía |
| Visitar Google, Wikipedia y Microsoft Learn | Microsoft Learn | Wikipedia → Google → Inicio | Vacía |
| Pulsar **Atrás** | Wikipedia | Google → Inicio | Microsoft Learn |
| Pulsar **Adelante** | Microsoft Learn | Wikipedia → Google → Inicio | Vacía |
| Pulsar **Atrás** y visitar `github.com` | GitHub | Wikipedia → Google → Inicio | Vacía |
| Pulsar **Reiniciar** | `Inicio` | Vacía | Vacía |

El paso de GitHub demuestra una regla importante: **al visitar una dirección nueva después de retroceder, se borra la pila Adelante**. Así se evita regresar a una secuencia de navegación que ya no corresponde al recorrido actual.

## Diseño de clases

| Clase | Responsabilidad | Elementos principales |
| --- | --- | --- |
| `HistorialNavegacion` | Implementar y proteger la lógica del historial. | `_atras`, `_adelante`, `PaginaActual`, `Visitar`, `Volver`, `Avanzar`, `Reiniciar`. |
| `FormularioHistorial` | Presentar el estado y gestionar la interacción en Windows Forms. | Campo de dirección, botones, listas visuales, registro y exportación de evidencias. |
| `Program` de `Aplicacion` | Iniciar la interfaz gráfica. | `ApplicationConfiguration.Initialize` y `Application.Run`. |
| `Program` de `Pruebas` | Ejecutar verificaciones independientes de la interfaz. | Doce casos y funciones auxiliares de comprobación. |

`FormularioHistorial` utiliza una instancia de `HistorialNavegacion`; no manipula directamente sus pilas. La clase del núcleo expone propiedades de solo lectura y copias del contenido mediante `ToArray`, de modo que la representación gráfica no pueda alterar los elementos internos. El diagrama de clases correspondiente también está incluido en el reporte de práctica.

## Pruebas automatizadas

El proyecto `Pruebas` comprueba la lógica sin abrir Windows Forms. Se ejecuta desde la carpeta principal con:

```powershell
dotnet run --project Pruebas/Pruebas.csproj -c Release
```

Incluye **12 casos de prueba**:

| Caso | Condición verificada |
| --- | --- |
| 1. Estado inicial | Página `Inicio` y ambas pilas vacías. |
| 2. Tres visitas y orden LIFO | Cantidad almacenada y página superior de Atrás. |
| 3. Atrás una vez | Retorno a Wikipedia y conservación de Microsoft Learn en Adelante. |
| 4. Adelante luego de volver | Recuperación de Microsoft Learn y vaciado de Adelante. |
| 5. Nueva URL tras volver | Eliminación de la posibilidad de avanzar. |
| 6. Página actual repetida | La misma dirección no agrega una entrada adicional. |
| 7. URL sin protocolo | Normalización automática a HTTPS. |
| 8. Dirección vacía | Rechazo de una entrada en blanco. |
| 9. Protocolo no admitido | Rechazo de `ftp://`. |
| 10. Límites de navegación | Operaciones imposibles sobre pilas vacías sin alterar el estado. |
| 11. Reiniciar | Vaciado de pilas y recuperación de `Inicio`. |
| 12. Copias de las vistas | Cambiar una copia no modifica el historial interno. |

Cada caso informa `CORRECTO` o `ERROR`; al finalizar se muestran las comprobaciones correctas y fallidas. El proceso devuelve un código de salida distinto de cero cuando existe algún fallo. **La ejecución de estas pruebas en Windows debe comprobarse antes de la entrega**; no se atribuyen resultados de una ejecución que no esté documentada.

## Complejidad y uso de memoria

Las pilas se implementan con `Stack<string>`, una colección genérica de C#. En condiciones habituales, `Push` tiene costo **O(1) amortizado** y `Pop` cuesta **O(1)**. Vaciar una pila con `Clear` puede recorrer sus elementos y tiene costo **O(n)**. Por ello, `Volver` y `Avanzar` realizan operaciones de pila de costo constante; `Visitar` también incluye la validación de la URL y el posible vaciado de Adelante.

El historial ocupa **O(n)** referencias para `n` entradas almacenadas, además de la memoria de las cadenas. Mostrar ambas pilas en las listas de Windows Forms requiere recorrerlas y copiar sus valores; ese trabajo de presentación es independiente de la operación de extracción o inserción de una pila. El historial no tiene un límite fijo de elementos, pero su capacidad depende de los recursos disponibles.

## Publicar el ejecutable para Windows

Para generar una publicación de 64 bits que incluya el entorno de ejecución .NET:

```powershell
dotnet publish Aplicacion/Aplicacion.csproj -c Release -r win-x64 --self-contained true -o Publicacion
```

El proyecto también incluye `Publicar_EXE_Windows.bat`, que publica en la carpeta `publicado` utilizando la opción de archivo único. No se incluye un ejecutable previamente compilado: el repositorio contiene los archivos fuente y las instrucciones para generarlo.

## Entrega y evidencia audiovisual

**Repositorio de GitHub:** pendiente de publicación.  
**Video público de YouTube:** pendiente de publicación.

El reporte de práctica contiene la justificación de las pilas, el diagrama de clases, los casos de prueba y el guion para la explicación. La evidencia audiovisual debe mostrar la pantalla del programa y la cámara web en la esquina inferior derecha. El video se publicará en el mismo canal utilizado para las demás actividades. Las capturas deben proceder de la ejecución real de la aplicación en Windows.

## Organización del proyecto

- `Nucleo/HistorialNavegacion.cs`: lógica de las dos pilas, validación de direcciones y estado de navegación.
- `Aplicacion/FormularioHistorial.cs`: interfaz Windows Forms y eventos de los controles.
- `Aplicacion/Program.cs`: punto de entrada de la aplicación gráfica.
- `Pruebas/Program.cs`: doce casos automatizados de la lógica.
- `Historial_Web.sln`: solución con los tres proyectos.
- `Ejecutar_en_Windows.bat`: ejecución de la aplicación.
- `Probar_logica.bat`: ejecución de las pruebas.
- `Publicar_EXE_Windows.bat`: publicación del ejecutable para Windows.
- `.gitignore`: exclusión de carpetas de compilación y archivos temporales de Visual Studio.

## Referencias

- Microsoft. *Stack\<T\> Class*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1
- Microsoft. *Windows Forms overview*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/
- Microsoft. *Uri Class*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/api/system.uri
