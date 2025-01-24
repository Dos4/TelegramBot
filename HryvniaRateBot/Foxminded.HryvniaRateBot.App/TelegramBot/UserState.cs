using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;

namespace Foxminded.HryvniaRateBot.App.TelegramBot;

public class UserState
{
    public long Id { get; set; }
    public CurrencyCode Currency { get; set; } = default;
    public DateOnly Date { get; set; } = default;
    public LanguageCode Language { get; set; }
}
