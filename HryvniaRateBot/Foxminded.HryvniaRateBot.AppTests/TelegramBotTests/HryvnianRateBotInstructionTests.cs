using Foxminded.HryvniaRateBot.App.DataAccess.Providers;
using Foxminded.HryvniaRateBot.App.DataAccess;
using Foxminded.HryvniaRateBot.App.Services;
using Foxminded.HryvniaRateBot.App.TelegramBot.Markups;
using Foxminded.HryvniaRateBot.App.TelegramBot;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot.Types;
using Foxminded.HryvniaRateBot.App.Resources;
using Telegram.Bot.Types.ReplyMarkups;
using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;

namespace Foxminded.HryvniaRateBot.AppTests.TelegramBotTests;

[TestClass]
public class HryvnianRateBotInstructionTests
{
    private Mock<IProvider>? _providerMock;
    private Mock<ILogger<HryvnianRateBotInstruction>>? _loggerMock;
    private Mock<IExchangeRateService>? _exchangeRateServiceMock;
    private Mock<IMessageService>? _messageServiceMock;
    private Mock<ITelegramCalendarMarkup>? _calendarMarkupMock;
    private HryvnianRateBotInstruction? _botInstruction;
    private ExchangeRateResponse? _exchangeRateResponse;

    [TestInitialize]
    public void Setup()
    {
        _providerMock = new Mock<IProvider>();
        _loggerMock = new Mock<ILogger<HryvnianRateBotInstruction>>();
        _messageServiceMock = new Mock<IMessageService>();
        _calendarMarkupMock = new Mock<ITelegramCalendarMarkup>();

        var loggerExchangeRateMock = new Mock<ILogger<ExchangeRateService>>();
        _exchangeRateServiceMock = new Mock<IExchangeRateService>();


        _botInstruction = new HryvnianRateBotInstruction(
            _exchangeRateServiceMock.Object,
            _calendarMarkupMock.Object,
            _loggerMock.Object,
            _messageServiceMock.Object, _providerMock.Object);
    }

    [TestMethod]
    public async Task GetResponseToStartCommand_ShouldSendWelcomeMessage()
    {
        var message = new Message { Chat = new Chat { Id = 12345 } };

        _messageServiceMock!
            .Setup(service => service.SendMessage(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IReplyMarkup>()))
            .Returns(Task.CompletedTask);

        await _botInstruction!.GetResponseToStartCommand(message);

        _messageServiceMock.Verify(service =>
            service.SendMessage(12345, BotMessage.WelcomeMessage, It.IsAny<IReplyMarkup>()), Times.Once);
    }

    [TestMethod]
    public async Task GetResponseToNewOperationCommand_ShouldResetUserStateAndSendMessage()
    {        var message = new Message
        {
            Chat = new Chat { Id = 12345 },
            From = new User { Id = 67890 }
        };

        _messageServiceMock!
            .Setup(service => service.SendMessage(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IReplyMarkup>()))
            .Returns(Task.CompletedTask);

        await _botInstruction!.GetResponseToNewOperationCommand(message);

        _messageServiceMock.Verify(service =>
            service.SendMessage(12345, BotMessage.StartNewOperation, It.IsAny<IReplyMarkup>()), Times.Once);
    }

    [TestMethod]
    public async Task HandleCallbackQueryAsync_ShouldHandleCurrencyCallback()
    {
        var query = new CallbackQuery
        {
            Data = "USD",
            From = new User { Id = 12345 },
            Id = "callback-id"
        };

        _messageServiceMock!
            .Setup(service => service.AnswerCallbackQuery(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await _botInstruction!.HandleCallbackQueryAsync(query);

        _messageServiceMock.Verify(service =>
            service.AnswerCallbackQuery("callback-id", It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task HandleCallbackQueryAsync_ShouldSendInvalidOptionMessage_OnUnknownCallbackData()
    {
        var query = new CallbackQuery
        {
            Data = "unknown-data",
            Id = "callback-id"
        };

        _messageServiceMock!
            .Setup(service => service.AnswerCallbackQuery(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await _botInstruction!.HandleCallbackQueryAsync(query);

        _messageServiceMock.Verify(service =>
            service.AnswerCallbackQuery("callback-id", BotMessage.InvalidOption), Times.Once);
    }
}
