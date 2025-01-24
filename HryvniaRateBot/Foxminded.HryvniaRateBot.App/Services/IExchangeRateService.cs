using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;
using Foxminded.HryvniaRateBot.App.TelegramBot;

namespace Foxminded.HryvniaRateBot.App.Services;

public interface IExchangeRateService
{
    public ExchangeRate GetExchangeRateForClientsCurrency(ExchangeRateResponse ratesByData, CurrencyCode usersCurrency);

    public string ChangeResultsForNBUInfromation(ExchangeRate result, UserState userState);
}
