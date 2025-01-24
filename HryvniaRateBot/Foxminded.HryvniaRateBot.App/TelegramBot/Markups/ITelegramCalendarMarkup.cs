using Telegram.Bot.Types;

namespace Foxminded.HryvniaRateBot.App.TelegramBot.Markups;

public interface ITelegramCalendarMarkup
{
    Task<string> HandleCalendarCallbackQuery(CallbackQuery query);
}
