using Foxminded.HryvniaRateBot.App.Exceptions;
using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;
using Foxminded.HryvniaRateBot.App.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Foxminded.HryvniaRateBot.App.DataAccess.Providers;

public class PrivatBankProvider : IProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiURL;
    private ILogger<PrivatBankProvider> _logger;

    public PrivatBankProvider(IOptions<PrivatBankOptions> options, HttpClient client, ILogger<PrivatBankProvider> logger)
    {
        _logger = logger ?? throw new ArgumentException();
        var config = options.Value ?? throw new ArgumentNullException(nameof(options));
        if (!Uri.TryCreate(config.ApiUrl, UriKind.Absolute, out var apiUri))
        {
            _logger.LogError($"Invalid API URL in configuration {nameof(options)}");
            throw new ArgumentException();
        }
        _httpClient = client ?? throw new ArgumentNullException();
        _apiURL = config.ApiUrl ?? throw new ArgumentNullException();
    }

    public async Task<ExchangeRateResponse> GetExchangeRatesAsync(DateOnly date)
    {
        var response = await _httpClient.GetAsync($"{_apiURL}{date}");
        _logger.LogInformation("Connection with API: {url}, connection code: {code}", _apiURL, response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to fetch data from API. URL: {_apiURL}, Status Code: {response.StatusCode}");
            throw new HttpRequestException();
        }

        var json = await response.Content.ReadAsStringAsync();

        _logger.LogInformation($"Content were succesfull received");

        var result = JsonConvert.DeserializeObject<ExchangeRateResponse>(json) ?? throw new FormatException();

        if (result == null || result.ExchangeRates.Length == 0)
            throw new EmptyBankInfoException();

        return result;
    }
}
