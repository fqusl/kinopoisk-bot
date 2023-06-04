using KinopoiskApiClient;

namespace KinopoiskBot.View;

public static class TextView
{
    public static string Convert<T>(T entity)
    {
        var properties = typeof(T)
            .GetProperties()
            .Where(x => x.Name != "Id")
            .Select(x => (x.Name, x.GetValue(entity).ToString()))
            .Select(x => $"{x.Name}: {x.Item2}");

        return string.Join("\n\n", properties);
    }
}