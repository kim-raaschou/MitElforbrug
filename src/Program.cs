using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MitElforbrug.Infrastructure;
using MitElforbrug.Features;
using MitElforbrug;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((Action<IServiceCollection>)(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<VisMålerOplysningerHandler>();
        services.AddScoped<VisElforbrugOgSpotpriserHandler>();

        services.AddHttpClient<EnerginetHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.energidataservice.dk");
        });

        AddEloverblikHttpClient(services);

    }))
    .Build();

host.Run();

static void AddEloverblikHttpClient(IServiceCollection services)
{
    var eloverblikBaseAddress = new Uri("https://api.eloverblik.dk/customerapi/api");
    services
        .AddHttpClient<EloverblikHttpClient>(
            client => client.BaseAddress = eloverblikBaseAddress)
        .AddHttpMessageHandler(
            services => new EloverblikJwtTokenHandler(
                timeProvider: services.GetRequiredService<TimeProvider>(),
                baseAddress: eloverblikBaseAddress
            )
        );
}