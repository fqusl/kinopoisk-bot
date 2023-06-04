using System.Text.Json;
using KinopoiskApiClient;
using KinopoiskBot.Configuration;
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

    public TelegramBot(BotConfiguration configuration, IKinopoiskApi kinopoiskApi, ILogger<TelegramBot> logger)
    {
        this.configuration = configuration;
        this.kinopoiskApi = kinopoiskApi;
        this.logger = logger;
        client = new TelegramBotClient(this.configuration.Token);
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
                Command = BotCommands.Random.ToString(),
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
        if (update.Type != UpdateType.Message)
            return;

        var message = update.Message;

        logger.LogDebug($"Received: username=[{update.Message.From.Username}] Text=[{update.Message.Text}]");

        if (message.Text.ToLower() == $"/{BotCommands.Start.ToString().ToLower()}")
        {
            await client.SendTextMessageAsync(message.Chat,
                "Этот бот позволяет получать информацию из кинопоиска", cancellationToken: cancellationToken);
            return;
        }

        if (message.Text.ToLower() == $"/{BotCommands.Random.ToString().ToLower()}")
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
        await Send(botClient, update, $"Слуйчайный фильм:\n\n{TextView.Convert(randomMovie)}", cancellationToken);
    }

    private async Task Send(ITelegramBotClient botClient, Update update, string reply,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(update.Message.Chat, reply,
            cancellationToken: cancellationToken);
    }
}