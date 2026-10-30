@echo off
cd /d "%~dp0"
echo Publicando programa para Windows de 64 bits...
dotnet publish Aplicacion\Aplicacion.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o publicado
if errorlevel 1 (
 echo No se pudo publicar. Verifica el SDK de .NET 8 y tu conexion si es necesaria para restaurar componentes.
) else (
 echo Ejecutable creado en la carpeta: publicado\HistorialWeb.exe
)
pause
