# Hydron

```powershell
dotnet new sln --name Hydron
dotnet new webapi --framework net10.0 --no-https --use-controllers --use-program-main --output src/WebApi --name Hydron.WebApi
dotnet add src/WebApi package Swashbuckle.AspNetCore.SwaggerUI
dotnet sln add src/WebApi
```
