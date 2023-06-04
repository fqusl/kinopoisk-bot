using KinopoiskApiClient.Models;

namespace KinopoiskApiClient;

public interface IKinopoiskApi
{
    Movie? GetRandomMovie();
}