using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;

namespace Foxminded.HryvniaRateBot.App.DataAccess.Providers;

public interface IProvider
{
    public Task<ExchangeRateResponse> GetExchangeRatesAsync(DateOnly date);
}
