using Foxminded.HryvniaRateBot.App.Resources;
using Telegram.Bot.Types.ReplyMarkups;

namespace Foxminded.HryvniaRateBot.App.TelegramBot.Markups;

public static class InlineKeyboardMarkupExtension
{
    public static InlineKeyboardMarkup GetMarkupForCurrencyCode()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsText.USDollarText, "USD"),
                InlineKeyboardButton.WithCallbackData(BotButtonsText.EURText, "EUR"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsText.CHFText, "CHF"),
                InlineKeyboardButton.WithCallbackData(BotButtonsText.GBPText, "GBP"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsText.PLNText, "PLN"),
                InlineKeyboardButton.WithCallbackData(BotButtonsText.SEKText, "SEK"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsText.XAUText, "XAU"),
                InlineKeyboardButton.WithCallbackData(BotButtonsText.CADText, "CAD"),
            }
        });
    }

    public static InlineKeyboardMarkup GetInlineMarkupForDateChoosing()
    {
        return new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData(BotButtonsText.CalendarButtonText, "calendar"),
                    InlineKeyboardButton.WithCallbackData(BotButtonsText.TodayButtonText, DateOnly.FromDateTime(DateTime.Now).ToString())});
    }

    public static InlineKeyboardMarkup GetInlineMakrupForCallingOnlyCalendar()
    {
        return new InlineKeyboardMarkup(new[] {
                        InlineKeyboardButton.WithCallbackData(BotButtonsText.CalendarButtonText, "calendar")});
    }
}
