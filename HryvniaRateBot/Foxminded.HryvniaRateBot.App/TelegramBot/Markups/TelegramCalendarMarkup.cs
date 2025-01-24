using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Foxminded.HryvniaRateBot.App.Resources;

namespace Foxminded.HryvniaRateBot.App.TelegramBot.Markups;

public class TelegramCalendarMarkup : ITelegramCalendarMarkup
{
    private const int yearMarkupRowsCount = 4;
    private const int monthMarkupRowsCount = 3;
    private const int dayMarkupRowsCount = 7;
    private TelegramBotClient _bot;

    public TelegramCalendarMarkup(TelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<string> HandleCalendarCallbackQuery(CallbackQuery query) 
    {
        var data = query.Data!.Split(':');
        var action = data[0];

        switch (action)
        {
            case "calendar":
                await _bot.EditMessageReplyMarkup(query.Message!.Chat.Id, query.Message.MessageId, replyMarkup: GetYearKeyboard());
                break;

            case "year":
                await GetYearCaseMakrup(data, query);
                break;

            case "month":
                await GetMonthCaseMakrup(data, query);
                break;

            case "day":
                return await GetDayCaseMakrup(data, query);

            default:
                await _bot.AnswerCallbackQuery(query.Id, BotMessage.InvalidOption);
                break;
        }
        return null!;
    }

    private async Task GetYearCaseMakrup(string[] data, CallbackQuery query)
    {
        var selectedYear = int.Parse(data[1]);
        await _bot.EditMessageReplyMarkup(query.Message!.Chat.Id, query.Message.MessageId,
            replyMarkup: GetMonthKeyboard(selectedYear));
    }
    private async Task GetMonthCaseMakrup(string[] data, CallbackQuery query)
    {
        var year = int.Parse(data[1]);
        var month = int.Parse(data[2]);
        await _bot.EditMessageReplyMarkup(query.Message!.Chat.Id, query.Message.MessageId,
            replyMarkup: GetDayKeyboard(year, month));
    }
    private async Task<string> GetDayCaseMakrup(string[] data, CallbackQuery query)
    {
        var selectedDate = new DateOnly(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
        await _bot.EditMessageText(query.Message!.Chat.Id, query.Message.MessageId,
            string.Format(BotMessage.PickDate, selectedDate), replyMarkup: InlineKeyboardMarkupExtension.GetInlineMarkupForDateChoosing());
        return selectedDate.ToString();
    }

    private InlineKeyboardMarkup GetYearKeyboard(int startYear = 2014)
    {
        var currentYear = DateTime.Now.Year;
        var buttons = new List<List<InlineKeyboardButton>>();
        var row = new List<InlineKeyboardButton>();

        for (int year = startYear; year <= currentYear; year++)
        {
            row.Add(InlineKeyboardButton.WithCallbackData(year.ToString(), $"year:{year}"));
            if (row.Count == yearMarkupRowsCount)
            {
                buttons.Add(row);
                row = new List<InlineKeyboardButton>();
            }
        }

        if (row.Count > 0)
            buttons.Add(row);

        return new InlineKeyboardMarkup(buttons);
    }

    private InlineKeyboardMarkup GetMonthKeyboard(int year)
    {
        var buttons = new List<List<InlineKeyboardButton>>();
        var row = new List<InlineKeyboardButton>();
        var months = new[] { BotButtonsText.JanuaryText, BotButtonsText.FebruaryText, BotButtonsText.MarchText, BotButtonsText.AprilText, BotButtonsText.MayText,
            BotButtonsText.JuneText, BotButtonsText.JulyText, BotButtonsText.AugustText, BotButtonsText.SeptemberText, BotButtonsText.OctoberText,
                BotButtonsText.NovemberText, BotButtonsText.DecemberText };

        for (int i = 0; i < months.Length; i++)
        {
            row.Add(InlineKeyboardButton.WithCallbackData(months[i], $"month:{year}:{i + 1}"));
            if (row.Count == monthMarkupRowsCount)
            {
                buttons.Add(row);
                row = new List<InlineKeyboardButton>();
            }
        }

        if (row.Count > 0)
            buttons.Add(row);

        return new InlineKeyboardMarkup(buttons);
    }

    private InlineKeyboardMarkup GetDayKeyboard(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var buttons = new List<List<InlineKeyboardButton>>();
        var row = new List<InlineKeyboardButton>();

        for (int day = 1; day <= daysInMonth; day++)
        {
            row.Add(InlineKeyboardButton.WithCallbackData(day.ToString(), $"day:{year}:{month}:{day}"));
            if (row.Count == dayMarkupRowsCount)
            {
                buttons.Add(row);
                row = new List<InlineKeyboardButton>();
            }
        }

        if (row.Count > 0)
            buttons.Add(row);

        return new InlineKeyboardMarkup(buttons);
    }
}
