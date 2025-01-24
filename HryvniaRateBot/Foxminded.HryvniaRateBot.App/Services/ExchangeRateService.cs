using Foxminded.HryvniaRateBot.App.Exceptions;
using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;
using Foxminded.HryvniaRateBot.App.Resources;
using Foxminded.HryvniaRateBot.App.TelegramBot;
using Microsoft.Extensions.Logging;

namespace Foxminded.HryvniaRateBot.App.Services;

public class ExchangeRateService : IExchangeRateService
{
    private ILogger<ExchangeRateService> _logger;

    public ExchangeRateService(ILogger<ExchangeRateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException();
    }

    public ExchangeRate GetExchangeRateForClientsCurrency(ExchangeRateResponse ratesByData, CurrencyCode usersCurrency)
    {
        ArgumentNullException.ThrowIfNull(ratesByData);
        ArgumentNullException.ThrowIfNull(usersCurrency);

        foreach (var exchangeRate in ratesByData.ExchangeRates)
        {
            Enum.TryParse<CurrencyCode>(exchangeRate.Currency, out var existingCurrency);
            if(existingCurrency == usersCurrency)
                return exchangeRate;
        }
        throw new EmptyBankInfoException();
    }

    public string ChangeResultsForNBUInfromation(ExchangeRate result, UserState userState)
        => string.Format(BotMessage.ExchangeRateInfo, result.Currency,
                result.SaleRateNationalBank.ToString("0.00"), result.PurchaseRateNationalBank.ToString("0.00"), userState.Date, BotMessage.NBUText);
}
