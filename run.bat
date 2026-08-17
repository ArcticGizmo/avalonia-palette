@echo off
REM Launch the Avalonia Palette sample app.
REM   run.bat            → build + run (Release)
REM   run.bat --verify   → headless WCAG contrast gate
setlocal
dotnet run --project "%~dp0src\Palette.Sample\Palette.Sample.csproj" -c Release -- %*
endlocal
