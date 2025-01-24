using Foxminded.HryvniaRateBot.App.Exceptions;
using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;
using Foxminded.HryvniaRateBot.App.Resources;
using Foxminded.HryvniaRateBot.App.Services;
using Foxminded.HryvniaRateBot.App.TelegramBot;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foxminded.HryvniaRateBot.AppTests.ServicesTests;

[TestClass]
public class ExchangeRateServiceTests
{
    private Mock<ILogger<ExchangeRateService>>? _loggerMock;
    private ExchangeRateService? _exchangeRateService;

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ExchangeRateService>>();
        _exchangeRateService = new ExchangeRateService(_loggerMock.Object);
    }

    [TestMethod]
    public void GetExchangeRateForClientsCurrency_ReturnsCorrectRate_WhenCurrencyExists()
    {
        var ratesResponse = new ExchangeRateResponse
        {
            ExchangeRates = new[]
        {
            new ExchangeRate { BaseCurrency = "UAH", Currency = "USD", SaleRate = 36.7m, PurchaseRate = 36.5m },
            new ExchangeRate { BaseCurrency = "UAH", Currency = "EUR", SaleRate = 39.5m, PurchaseRate = 39.3m }
        }
        };
        var usersCurrency = CurrencyCode.USD;

        var result = _exchangeRateService!.GetExchangeRateForClientsCurrency(ratesResponse, usersCurrency);

        Assert.AreEqual("USD", result.Currency);
        Assert.AreEqual(36.7m, result.SaleRate);
        Assert.AreEqual(36.5m, result.PurchaseRate);
    }

    [TestMethod]
    public void ChangeResultsForNBUInfromation_ReturnsFormattedString_WhenCalledWithValidData()
    {
        var exchangeRate = new ExchangeRate { BaseCurrency = "UAH", Currency = "USD", SaleRateNationalBank = 36.5m, PurchaseRateNationalBank = 36.3m };
        var userState = new UserState { Date = new DateOnly(2025,01,10) };

        var result = _exchangeRateService!.ChangeResultsForNBUInfromation(exchangeRate, userState);

        var expectedMessage = BotMessage.ExchangeRateInfo;
        Assert.AreEqual(string.Format(expectedMessage, exchangeRate.Currency,
                exchangeRate.SaleRateNationalBank.ToString("0.00"), exchangeRate.PurchaseRateNationalBank.ToString("0.00"), userState.Date, BotMessage.NBUText), result);
    }

    [TestMethod]
    [ExpectedException(typeof(EmptyBankInfoException))]
    public void GetExchangeRateForClientsCurrency_ThrowsException_WhenCurrencyDoesNotExist()
    {
        var ratesResponse = new ExchangeRateResponse
        {
            ExchangeRates = new[]
            {
            new ExchangeRate { BaseCurrency = "UAH", Currency = "EUR", SaleRate = 39.5m, PurchaseRate = 39.3m }
        }
        };
        var usersCurrency = CurrencyCode.USD;

        _exchangeRateService!.GetExchangeRateForClientsCurrency(ratesResponse, usersCurrency);
    }
}