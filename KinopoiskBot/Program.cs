// See https://aka.ms/new-console-template for more information

using KinopoiskApiClient;
using KinopoiskBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var configFileName = "appsettings.json";
var serviceProvider = ConfigureContainer();
var bar = serviceProvider.GetService<IBot>();
bar.Run();


ServiceProvider ConfigureContainer()
{
    var config = BuildCConfiguration();
    var botConfig = GetConfiguration<BotConfiguration>(config);
    var kinopoiskApiConfig = GetConfiguration<KinopoiskApiConfiguration>(config);

    return new ServiceCollection()
        .AddLogging(c => c.SetMinimumLevel(LogLevel.Debug).AddConsole())
        .AddSingleton<IBot, TelegramBot>()
        .AddSingleton(botConfig)
        .AddKinopoisk(kinopoiskApiConfig)
        .BuildServiceProvider();
}

IConfiguration BuildCConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(configFileName, optional: false);
    return builder.Build();
}

T GetConfiguration<T>(IConfiguration configuration)
{
    var config = configuration.GetSection(typeof(T).Name).Get<T>();
    if (config is null)
        throw new ApplicationException($"Не настроен {typeof(T).Name} в {configFileName}");
    return config;
}