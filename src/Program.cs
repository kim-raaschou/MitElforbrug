using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MitElforbrug.Infrastructure;
using MitElforbrug.Core.Features;

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
