using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GoldFetchTimer.HelperClass;
using Microsoft.Extensions.Logging;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Register HttpClientFactory
        services.AddHttpClient();

        // Register configuration
        var configuration = context.Configuration;

        // Register GoldApiFetch
        services.AddSingleton<GoldApiFetch>();

        // Register GoldHistoryRepository with connection string
        services.AddScoped<IGoldHistoryRepository>(provider =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var apiFetch = provider.GetRequiredService<GoldApiFetch>();
            var logger = provider.GetRequiredService<ILogger<GoldHistoryRepository>>();

            return new GoldHistoryRepository(connectionString, apiFetch, logger);
        });

        // Register other services as needed
    })
    .Build();

host.Run();