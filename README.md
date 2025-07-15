# Base de Azure Function en modo aislado

Este repositorio contiene una plantilla básica para crear proyectos de [Azure Functions](https://learn.microsoft.com/azure/azure-functions/) utilizando el modelo **Isolated Worker** con .NET 8. La solución incluye dos proyectos principales:

1. **BussinesLogic** – Biblioteca de clases donde se implementa la lógica de negocio reutilizable. Un ejemplo es `ExcelProcessor`, que utiliza la librería **ClosedXML** para manipular archivos de Excel.
2. **ProyectoUI** – Proyecto de funciones de Azure que referencia a `BussinesLogic` y define las funciones desencadenadoras. Se configura con paquetes de autenticación, registro mediante Serilog y las extensiones de Azure Functions para HTTP y temporizadores.

## Tecnologías utilizadas

- **.NET 8**
- **Azure Functions v4** en modo aislado (`Microsoft.Azure.Functions.Worker`)
- **Serilog** para el registro de eventos
- **Application Insights** (opcional) para telemetría
- **ClosedXML** para manejo de Excel en la capa de negocio

Consulta los archivos `.csproj` de cada proyecto para ver todas las dependencias utilizadas.

## ¿Para qué sirve este código?

El objetivo es ofrecer una base lista para iniciar nuevos proyectos de Azure Functions en modo aislado. Incluye la configuración de dependencias, middleware de ejemplo y una función programada (`SincronizeFunction`) que se ejecuta cada hora. Puedes modificar o ampliar esta base para construir tus propios servicios sin servidor en Azure.

## Requisitos previos

- [.NET SDK 8.0](https://dotnet.microsoft.com/) instalado
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) para ejecutar y depurar funciones de forma local

## Ejecución local

1. Restaura las dependencias y compila la solución:
   ```bash
   dotnet build BaseAzureFunction.sln
   ```
2. Ve al proyecto de funciones y ejecuta:
   ```bash
   cd ProyectoUI
   func start
   ```
   También puedes usar `dotnet run` si lo prefieres.

> **Nota:** los archivos `__azurite_db_*` incluidos son las bases de datos locales generadas por Azurite (emulador de Azure Storage) para pruebas locales.

## Personalización

- Agrega tus propias funciones creando clases con el atributo `[Function("Nombre")]`.
- Usa `appsettings.json` o `local.settings.json` (excluido por `.gitignore`) para configurar cadenas de conexión y otros valores.
- Modifica `host.json` para ajustar la configuración global de Azure Functions.

## Despliegue

1. Publica el proyecto en modo *Release*:
   ```bash
   dotnet publish ProyectoUI/ProyectoUI.csproj -c Release
   ```
2. Inicia sesión en tu suscripción de Azure desde la consola:
   ```bash
   func azure login
   ```
   También puedes hacerlo con `az login` si tienes [Azure CLI](https://learn.microsoft.com/cli/azure/) instalado.
3. Asegúrate de contar con una Function App creada (o créala previamente con `az functionapp create`).
4. Utiliza Functions Core Tools para publicar directamente:
   ```bash
   func azure functionapp publish <NombreDeLaFunctionApp> --csharp
   ```
   Esto enviará el contenido de `ProyectoUI/bin/Release/net8.0/publish` a tu aplicación en Azure.

Esta plantilla sirve como punto de partida para tus proyectos de funciones en .NET. Personalízala según tus necesidades y amplía la lógica de negocio en el proyecto `BussinesLogic`.
