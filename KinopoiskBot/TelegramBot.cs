using Telegram.Bot;

namespace KinopoiskBot;

public class TelegramBot : IBot
{
    private readonly BotConfiguration configuration;

    public TelegramBot(BotConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GetMessage()
    {
        var botClient = new TelegramBotClient(configuration.Token);

        var me = botClient.GetUpdatesAsync().Result[0];
        return $"Hello, World! I am user {me.Id} and my name is {me.Message} {me.Message.From.Username}.";
    }
}