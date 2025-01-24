using Foxminded.HryvniaRateBot.App.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Foxminded.HryvniaRateBot.App.TelegramBot;

public class HryvnianRateBot
{
    private readonly TelegramBotClient _bot;
    private ILogger<HryvnianRateBot> _logger;
    private HryvnianRateBotInstruction _instruction;

    public HryvnianRateBot(TelegramBotClient telegramBot, ILogger<HryvnianRateBot> logger, HryvnianRateBotInstruction instruction)
    {
        _bot = telegramBot;
        _logger = logger;
        _instruction = instruction;

        _bot.StartReceiving(
            updateHandler: async (botClient, update, cancellationToken) =>
                { await HandleUpdateAsync(update); },
            errorHandler: async (botClient, exception, cancellationToken) =>
                { await HandleErrorAsync(exception); }
        );
    }

    private async Task HandleErrorAsync(Exception exception)
    {
        _logger.LogError(exception.Message);
        _logger.LogDebug(exception.ToString());
    }

    private async Task HandleUpdateAsync(Update update)
    {
        var user = update.Message?.From ?? update.CallbackQuery?.From;

        LocalizationManager.SetLanguageForUser(user!, _logger);

        _logger.LogDebug($"User: {user!.Username}, Id: {user.Id}, Language: {user.LanguageCode}, " +
            $"Update Type: {update.Type}, Content: {update.CallbackQuery?.Data ?? update.Message?.Text ?? "N/A"}");

        switch (update.Type)
        {
            case UpdateType.CallbackQuery:
                await _instruction.HandleCallbackQueryAsync(update.CallbackQuery!);
                break;

            case UpdateType.Message:
                await HandleMessageAsync(update.Message!);
                break;

            default:
                _logger.LogWarning($"Unhandled update type: {update.Type}");
                break;
        }
    }

    private async Task HandleMessageAsync(Message msg)
    {
        switch (msg.Text)
        {
            case "/start":
                await _instruction.GetResponseToStartCommand(msg);
                break;

            case "/new_operation":
                await _instruction.GetResponseToNewOperationCommand(msg);
                break;

            default:
                await _bot.SendMessage(msg.Chat.Id, BotMessage.UnknownCommand);
                break;
        }
    }
}
