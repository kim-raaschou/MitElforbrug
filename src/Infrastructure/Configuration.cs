﻿using Microsoft.Extensions.DependencyInjection;

namespace MitElforbrug.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection ConfigureEloverblikHttpClient(this IServiceCollection services)
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

        return services
            .AddSingleton(TimeProvider.System);
    }
}