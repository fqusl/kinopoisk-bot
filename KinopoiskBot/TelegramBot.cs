using System.Text.Json;
using KinopoiskApiClient;
using KinopoiskBot.View;
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

        var commands = new BotCommand[]
        {
            new()
            {
                Command = "random",
                Description = "Получить случайный фильм"
            }
        };

        Task.Run(() => client.SetMyCommandsAsync(commands, cancellationToken: cancellationToken), cancellationToken);

        client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );


        Console.ReadLine();

        cts.Cancel();
    }

    private async void HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message) return;
        var message = update.Message;
        if (message.Text.ToLower() == "/start")
        {
            await client.SendTextMessageAsync(message.Chat,
                "Этот бот позволяет получать информацию из кинопоиска", cancellationToken: cancellationToken);
            return;
        }

        if (message.Text.ToLower() == "/random")
        {
            await HandleRandomMovie(botClient, update, cancellationToken);
        }
    }

    private void HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(JsonSerializer.Serialize(exception.Message));
    }

    private async Task HandleRandomMovie(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var randomMovie = kinopoiskApi.GetRandomMovie();
        await Send(botClient, update, $"Слуйчайный фильм:\n{TextView.Convert(randomMovie)}", cancellationToken);
    }

    private async Task Send(ITelegramBotClient botClient, Update update, string reply,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(update.Message.Chat, reply,
            cancellationToken: cancellationToken);
    }
}