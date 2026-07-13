using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace AMR.Core.API.Telemetry;

public static class TelemetryExtensions
{
    public static IServiceCollection AddAmrTelemetry(
        this IServiceCollection services,
        IConfiguration config,
        string serviceName,
        string serviceVersion = "3.0.0")
    {
        var otlpEndpoint = config["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";
        var environment = config["ASPNETCORE_ENVIRONMENT"] ?? "Production";

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion: serviceVersion)
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment,
            });

        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation(opt =>
                {
                    opt.RecordException = true;
                    opt.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(opt =>
                {
                    opt.SetDbStatementForText = true;
                    opt.SetDbStatementForStoredProcedure = true;
                })
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpEndpoint)))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpEndpoint)));

        return services;
    }
}
