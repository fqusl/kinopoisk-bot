using KinopoiskApiClient;
using KinopoiskApiClient.Configuration;

namespace KinopoiskBot.Configuration;

public class KinopoiskApiConfiguration : IKinopoiskApiConfiguration
{
    public string ApiKey { get; set; }
    public string Url { get; set; }
}