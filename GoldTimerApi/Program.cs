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
      
        services.AddHttpClient();
        var configuration = context.Configuration;

        services.AddSingleton<GoldApiFetch>();

        services.AddScoped<IGoldHistoryRepository>(provider =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var apiFetch = provider.GetRequiredService<GoldApiFetch>();
            var logger = provider.GetRequiredService<ILogger<GoldHistoryRepository>>();

            return new GoldHistoryRepository(connectionString, apiFetch, logger);
        });

    })
    .Build();

host.Run();