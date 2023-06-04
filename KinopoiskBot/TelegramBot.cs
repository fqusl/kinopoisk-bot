using System.Text.Json;
using KinopoiskApiClient;
using KinopoiskBot.View;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KinopoiskBot;

public class TelegramBot : IBot
{
    private readonly BotConfiguration configuration;
    private readonly IKinopoiskApi kinopoiskApi;
    private readonly TelegramBotClient client;
    private readonly ILogger logger;

    public TelegramBot(BotConfiguration configuration, IKinopoiskApi kinopoiskApi)
    {
        this.configuration = configuration;
        this.kinopoiskApi = kinopoiskApi;
        client = new TelegramBotClient(this.configuration.Token);
        logger = new LoggerFactory().CreateLogger("bot");
    }

    public void Run()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        
        client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        
        Console.ReadLine();

        cts.Cancel();
    }

    public string GetMessage()
    {
        var me = client.GetUpdatesAsync().Result[0];
        return $"Hello, World! I am user {me.Id} and my name is {me.Message} {me.Message.From.Username}.";
    }

    private async void HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message) return;
        var message = update.Message;
        if (message.Text.ToLower() == "/start")
        {
            await botClient.SendTextMessageAsync(message.Chat,
                "Этот бот позволяет получать информацию из кинопоиска");
            return;
        }

        var randomMovie = kinopoiskApi.GetRandomMovie();
        
        await botClient.SendTextMessageAsync(message.Chat, $"Слуйчайный фильм:\n{TextView.Convert(randomMovie)}");
    }

    private void HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(JsonSerializer.Serialize(exception.Message));
    }
}