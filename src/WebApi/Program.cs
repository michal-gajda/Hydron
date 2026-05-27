namespace Hydron.WebApi;

using Hydron.Application;
using Hydron.Infrastructure;

public sealed class Program
{
    private Program()
    {
    }

    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddObservability(serviceName: "Hydron.WebApi", serviceNamespace: "Hydron");

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopped.Register(() =>
        {
            if (Environment.ExitCode == 130 || Environment.ExitCode == 0)
            {
                Environment.Exit(0);
            }
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Hydron API v1");
            });
        }

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();

        return Environment.ExitCode;
    }
}
