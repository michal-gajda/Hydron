# TDD

## Functional Tests

### Nugets

- Microsoft.Extensions.Configuration
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- Shouldly

### References

- src\Infrastructure

### TestBase class

```csharp
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Application;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public abstract class TestBase : IDisposable
{
    private bool isDisposed;
    protected ServiceProvider ServiceProvider { get; private set; }

    protected TestBase()
    {
        var services = new ServiceCollection();

        var collection = new Dictionary<string, string?>
        {

        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(collection)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddLogging(cfg =>
        {
            cfg.AddDebug();
            cfg.SetMinimumLevel(LogLevel.Trace);
        });

        this.ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.ServiceProvider?.Dispose();
            this.ServiceProvider = null!;
        }

        this.isDisposed = true;
    }
}
```
