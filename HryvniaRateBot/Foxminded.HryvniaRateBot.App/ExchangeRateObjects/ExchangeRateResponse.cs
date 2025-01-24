using Newtonsoft.Json;

namespace Foxminded.HryvniaRateBot.App.ExchangeRateObjects;

public class ExchangeRateResponse
{
    public string? Date { get; set; }

    public string? Bank { get; set; }

    [JsonProperty("exchangeRate")]
    public required ExchangeRate[] ExchangeRates { get; set; }
}
