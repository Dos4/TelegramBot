using Foxminded.HryvniaRateBot.App.DataAccess;
using Foxminded.HryvniaRateBot.App.DataAccess.Providers;
using Foxminded.HryvniaRateBot.App.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace Foxminded.HryvniaRateBot.AppTests.DataAccessTests.ProvidersTests;

[TestClass]
public class PrivatBankProviderTests
{
    private MockHttpMessageHandler? _mockHttp;
    private HttpClient? _httpClient;
    private IConfiguration? _configuration;
    private Mock<ILogger<PrivatBankProvider>>? _loggerMock;
    private PrivatBankProvider? _provider;

    [TestInitialize]
    public void Setup()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttp);
        var testOptions = Options.Create(new PrivatBankOptions
        {
            ApiUrl = "http://localhost/api/rates/"
        });

        _loggerMock = new Mock<ILogger<PrivatBankProvider>>();
        _provider = new PrivatBankProvider(testOptions, _httpClient, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetExchangeRatesAsync_ShouldReturnRates_WhenApiResponseIsValid()
    {
        var date = new DateOnly(2025, 1, 1);
        var responseJson = @"{
            'date': '2025-01-01',
            'bank': 'PrivatBank',
            'exchangeRate': [
                { 'baseCurrency': 'UAH', 'currency': 'USD', 'saleRateNB': 37.2, 'purchaseRateNB': 36.8 }
            ]
        }";

        _mockHttp!.When($"http://localhost/api/rates/{date}")
                 .Respond("application/json", responseJson);

        var result = await _provider!.GetExchangeRatesAsync(date);

        Assert.IsNotNull(result);
        Assert.AreEqual("PrivatBank", result.Bank);
        Assert.AreEqual(1, result.ExchangeRates.Length);
        Assert.AreEqual("USD", result.ExchangeRates[0].Currency);
        Assert.AreEqual(37.2m, result.ExchangeRates[0].SaleRateNationalBank);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task GetExchangeRatesAsync_ShouldThrowHttpRequestException_WhenApiResponseIsUnsuccessful()
    {
        var date = new DateOnly(2025, 1, 1);
        _mockHttp!.When($"http://localhost/api/rates/{date}")
                 .Respond(HttpStatusCode.BadRequest);

        await _provider!.GetExchangeRatesAsync(date);
    }

    [TestMethod]
    [ExpectedException(typeof(EmptyBankInfoException))]
    public async Task GetExchangeRatesAsync_ShouldThrowEmptyBankInfoException_WhenExchangeRatesAreEmpty()
    {
        var date = new DateOnly(2025, 1, 1);
        var responseJson = @"{
        'date': '2025-01-01',
        'bank': 'PrivatBank',
        'exchangeRate': []
        }";

        _mockHttp!.When($"http://localhost/api/rates/{date}")
                 .Respond("application/json", responseJson);

        await _provider!.GetExchangeRatesAsync(date);
    }
}
