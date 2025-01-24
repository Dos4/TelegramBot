using Telegram.Bot.Types.ReplyMarkups;

namespace Foxminded.HryvniaRateBot.App.Services;

public interface IMessageService
{
    public Task<int> SendOrEditMessageAsync(long chatId, int? messageId, string text, InlineKeyboardMarkup? replyMarkup = null);

    public Task SendMessage(long chatId, string text, IReplyMarkup? replyMarkup = null);

    public Task AnswerCallbackQuery(string callbackQueryId, string text);
}
