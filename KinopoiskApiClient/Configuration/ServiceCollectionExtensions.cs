using Microsoft.Extensions.DependencyInjection;

namespace KinopoiskApiClient.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKinopoisk(this IServiceCollection services,
        IKinopoiskApiConfiguration configuration)
    {
        services
            .AddSingleton<IKinopoiskApi, KinopoiskApi>()
            .AddSingleton(configuration);
        
        return services;
    }
}