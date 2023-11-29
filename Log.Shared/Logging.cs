using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace Log.Shared;

public static class Logging
{
    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging() => (builderContext, loggerConfiguration) =>
    {
        var environment = builderContext.HostingEnvironment;

        loggerConfiguration
            .ReadFrom.Configuration(builderContext.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("Env", environment.EnvironmentName)
            .Enrich.WithProperty("AppName", environment.ApplicationName);

        var elasticSearchBaseUrl = builderContext.Configuration.GetSection("Elasticsearch")["BaseUrl"]!;
        var userName = builderContext.Configuration.GetSection("Elasticsearch")["UserName"]!;
        var password = builderContext.Configuration.GetSection("Elasticsearch")["Password"]!;
        var indexName = builderContext.Configuration.GetSection("Elasticsearch")["IndexName"]!;

        loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchBaseUrl))
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
            IndexFormat = $"{indexName}-{environment.EnvironmentName}-logs-" + "{0:yyy.MM.dd}",
            ModifyConnectionSettings = x => x.BasicAuthentication(userName, password),
            CustomFormatter = new ElasticsearchJsonFormatter()
        });
    };
}