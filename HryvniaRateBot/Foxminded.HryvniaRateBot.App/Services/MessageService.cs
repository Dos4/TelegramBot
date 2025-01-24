using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace Foxminded.HryvniaRateBot.App.Services;

public class MessageService : IMessageService
{
    private readonly TelegramBotClient _bot;

    public MessageService(TelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<int> SendOrEditMessageAsync(long chatId, int? messageId, string text, InlineKeyboardMarkup? replyMarkup = null)
    {
        if (messageId.HasValue)
        {
            await _bot.EditMessageText(chatId, messageId.Value, text, replyMarkup: replyMarkup);
            return messageId.Value;
        }
        else
        {
            var sentMessage = await _bot.SendMessage(chatId, text, replyMarkup: replyMarkup);
            return sentMessage.MessageId;
        }
    }

    public async Task SendMessage(long chatId, string text, IReplyMarkup? replyMarkup = null)
        => await _bot.SendMessage(chatId, text, replyMarkup: replyMarkup);

    public async Task AnswerCallbackQuery(string callbackQueryId, string text)
        => await _bot.AnswerCallbackQuery(callbackQueryId, text);
}
