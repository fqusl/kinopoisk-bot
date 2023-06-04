namespace KinopoiskApiClient.Configuration;

public interface IKinopoiskApiConfiguration
{
    public string ApiKey { get; set; }
    public string Url { get; set; }
}