namespace Hydron.Application.FunctionalTests;

using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Application;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public abstract class TestBase : IDisposable
{
    public required TestContext TestContext { get; init; } // public TestContext TestContext { get; set; } = null!;
    protected ServiceProvider ServiceProvider { get; private set; }
    private bool isDisposed;

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

        services.AddLogging(cfg =>
        {
            cfg.AddDebug();
            cfg.SetMinimumLevel(LogLevel.Trace);
        });

        services.AddSingleton(TimeProvider.System);

        services.AddApplication();
        services.AddInfrastructure(configuration);

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
