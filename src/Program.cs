using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MitElforbrug.Infrastructure;
using MitElforbrug.Features;
using MitElforbrug;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<VisMålerOplysningerHandler>();
        services.AddScoped<VisElforbrugOgSpotpriserHandler>();

        services.AddHttpClient<EnerginetHttpClient>(client =>
        {
            client.BaseAddress = new EnerginetHttpClientBaseAddress();
        });

        services.AddSingleton<EloverblikHttpClientBaseAddress>();
        services.AddScoped<EloverblikJwtTokenHandler>();
        services.AddHttpClient<EloverblikHttpClient>(client =>
        {
            client.BaseAddress = new EloverblikHttpClientBaseAddress();
        }).AddHttpMessageHandler<EloverblikJwtTokenHandler>();

    })
    .Build();

host.Run();