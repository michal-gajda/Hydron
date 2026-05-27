namespace Hydron.WebApi;

using Hydron.Telemetry;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

internal static class ServiceExtensions
{
    private const string SERVICE_VERSION = "1.0.0";
    private const string SERVICE_INSTANCE_ID = "development";

    public static void AddObservability(this WebApplicationBuilder builder, string serviceName, string serviceNamespace)
    {
        Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator([
            new TraceContextPropagator(),
            new BaggagePropagator(),
        ]));

        builder.Services
            .AddHealthChecks();

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceNamespace, SERVICE_VERSION, autoGenerateServiceInstanceId: false, SERVICE_INSTANCE_ID);

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.SetResourceBuilder(resourceBuilder);
            options.AddOtlpExporter();
        });

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(resourceBuilder)
                .SetSampler(new AlwaysOnSampler())
                .AddHydronInstrumentation()
                .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                .AddHttpClientInstrumentation(options => options.RecordException = true)
                .AddOtlpExporter()
            )
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resourceBuilder)
                .AddHydronInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter((_, options) => options.TemporalityPreference = MetricReaderTemporalityPreference.Delta)
            );
    }

    private static TracerProviderBuilder AddHydronInstrumentation(this TracerProviderBuilder tracing)
    {
        return tracing.AddSource(HydronTelemetry.ActivitySourceName);
    }

    private static MeterProviderBuilder AddHydronInstrumentation(this MeterProviderBuilder metrics)
    {
        return metrics.AddMeter(HydronTelemetry.MeterName);
    }
}
