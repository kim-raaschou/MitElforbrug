using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MitElforbrug.Core.UseCases;
using MitElforbrug.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        services.ConfigureEloverblikHttpClient();
        services.AddScoped<VisMålerOplysningerHandler>();
    })
    .Build();

host.Run();
