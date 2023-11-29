using System.Diagnostics;
using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Shared;

public static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenTelemetryConstants>(configuration.GetSection("OpenTelemetry"));
        var openTelemetryConstants = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>()!;

        services.AddOpenTelemetry().WithTracing(options =>
        {
            options
                .AddSource(openTelemetryConstants.ActivitySourceName)
                .AddSource(DiagnosticHeaders.DefaultListenerName)
                .ConfigureResource(resource =>
                {
                    resource.AddService(openTelemetryConstants.ServiceName,
                        serviceVersion: openTelemetryConstants.ServiceVersion);
                });
            
            options.AddAspNetCoreInstrumentation(aspnetcoreOptions =>
            {
                aspnetcoreOptions.Filter = context =>
                    context.Request.Path.Value!.Contains("api", StringComparison.OrdinalIgnoreCase);
            });
            
            options.AddEntityFrameworkCoreInstrumentation(efCoreOptions =>
            {
                efCoreOptions.SetDbStatementForText = true;
            });
            
            options.AddHttpClientInstrumentation(httpOptions =>
            {
                var requestContent = "empty";
                httpOptions.EnrichWithHttpRequestMessage = async (activity, request) =>
                {
                    if (request.Content != null)
                    {
                        requestContent = await request.Content.ReadAsStringAsync();
                    }

                    activity.SetTag("http.request.body", requestContent);
                };
                httpOptions.EnrichWithHttpResponseMessage = async (activity, response) =>
                {
                    activity.SetTag("http.response.body", await response.Content.ReadAsStringAsync());
                };
                httpOptions.FilterHttpRequestMessage = request => !request.RequestUri!.AbsoluteUri.Contains("9200");
            });
            
            options.AddRedisInstrumentation(redisOptions => { redisOptions.SetVerboseDatabaseStatements = true; });
            
            options.AddOtlpExporter(builder => builder.Endpoint = new Uri(openTelemetryConstants.OtelCollectorBaseUrl));
        });
        
        ActivitySourceProvider.Source = new ActivitySource(openTelemetryConstants.ActivitySourceName);
    }
}