namespace Hydron.Application.FunctionalTests;

using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Application;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public abstract class ExtendedTestBase : IDisposable
{
    public required TestContext TestContext { get; init; } // public TestContext TestContext { get; set; } = null!;
    protected ServiceProvider ServiceProvider { get => services.BuildServiceProvider(); }
    protected ServiceCollection Services => this.services;
    private readonly ServiceCollection services = new();
    private bool isDisposed;

    protected ExtendedTestBase()
    {
        var collection = new Dictionary<string, string?>
        {

        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(collection)
            .Build();
        this.services.AddSingleton<IConfiguration>(configuration);

        this.services.AddApplication();
        this.services.AddInfrastructure(configuration);

        this.services.AddLogging(cfg =>
        {
            cfg.AddDebug();
            cfg.SetMinimumLevel(LogLevel.Trace);
        });
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
            this.services?.Clear();
        }

        this.isDisposed = true;
    }
}
