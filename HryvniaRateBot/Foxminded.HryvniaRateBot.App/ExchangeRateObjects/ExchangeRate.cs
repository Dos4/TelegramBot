using Newtonsoft.Json;

namespace Foxminded.HryvniaRateBot.App.ExchangeRateObjects;

public class ExchangeRate
{
    public required string BaseCurrency { get; set; }

    public required string Currency { get; set; }

    [JsonProperty("saleRateNB")]
    public decimal SaleRateNationalBank {  get; set; }

    [JsonProperty("purchaseRateNB")]
    public decimal PurchaseRateNationalBank { get; set; }

    public decimal SaleRate { get; set; }

    public decimal PurchaseRate { get; set; }
}
