using System.Text.Json;

namespace KinopoiskApiClient;

public class KinopoiskApi : IKinopoiskApi
{
    private readonly IKinopoiskApiConfiguration configuration;
    private readonly string RandomMovieUrl;
    private readonly HttpClient client;

    public KinopoiskApi(IKinopoiskApiConfiguration configuration)
    {
        this.configuration = configuration;
        RandomMovieUrl = $"{configuration.Url}/v1.3/movie/random";

        client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", configuration.ApiKey);
    }

    public Movie? GetRandomMovie()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, RandomMovieUrl);
        var response = SendAndGetResponse(request);

        return Deserialize<Movie>(response);
    }

    private string SendAndGetResponse(HttpRequestMessage requestMessage)
    {
        var response = client.Send(requestMessage);
        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"Ошибка при получение ответа от Кинопоиска: " +
                $"HttpStatus=[{response.StatusCode}] " +
                $"Message=[{response.RequestMessage}]"
            );

        var dataStream = response.Content.ReadAsStream();
        var reader = new StreamReader(dataStream);
        return reader.ReadToEnd();
    }

    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}