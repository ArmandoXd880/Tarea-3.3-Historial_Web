@echo off
cd /d "%~dp0"
where dotnet >nul 2>nul
if errorlevel 1 (
  echo No se encontro .NET. Instala el SDK de .NET 8 antes de continuar.
  pause
  exit /b 1
)
dotnet run --project Aplicacion\Aplicacion.csproj
if errorlevel 1 echo No se pudo iniciar. Comprueba que .NET 8 y los componentes de escritorio esten instalados.
pause
