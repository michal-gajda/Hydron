# Hydron

```powershell
dotnet new sln --name Hydron
dotnet new webapi --framework net10.0 --no-https --use-controllers --use-program-main --output src/WebApi --name Hydron.WebApi
dotnet add src/WebApi package Swashbuckle.AspNetCore.SwaggerUI
dotnet sln add src/WebApi
```

```powershell
dotnet new classlib --framework net10.0 --output src/Domain --name Hydron.Domain
dotnet sln add src/Domain
dotnet new classlib --framework net10.0 --output src/Application --name Hydron.Application
dotnet sln add src/Application
dotnet add src/Application reference src/Domain
dotnet new classlib --framework net10.0 --output src/Infrastructure --name Hydron.Infrastructure
dotnet sln add src/Infrastructure
dotnet add src/Infrastructure reference src/Application
dotnet add src/WebApi reference src/Infrastructure
```

```powershell
dotnet add src/Application package AutoMapper
dotnet add src/Application package FluentResults
dotnet add src/Application package MediatR
dotnet add src/Application package FluentValidation.DependencyInjectionExtensions
dotnet add src/Infrastructure package Microsoft.Extensions.Configuration.Binder
```

```powershell
dotnet new classlib --framework net10.0 --output src/Telemetry --name Hydron.Telemetry
dotnet sln add src/Telemetry
dotnet add src/Application reference src/Telemetry
```
