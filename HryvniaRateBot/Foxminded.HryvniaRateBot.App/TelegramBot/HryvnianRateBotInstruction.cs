using Foxminded.HryvniaRateBot.App.DataAccess.Providers;
using Foxminded.HryvniaRateBot.App.Exceptions;
using Foxminded.HryvniaRateBot.App.ExchangeRateObjects;
using Foxminded.HryvniaRateBot.App.Resources;
using Foxminded.HryvniaRateBot.App.Services;
using Foxminded.HryvniaRateBot.App.TelegramBot.Markups;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Foxminded.HryvniaRateBot.App.TelegramBot;

public class HryvnianRateBotInstruction
{
    private ILogger<HryvnianRateBotInstruction> _logger;
    private IProvider _bankProvider;
    private UserState _userState;
    private CancellationTokenSource _currentCts;
    private Dictionary<long, int?> _userLastMessageId;
    private ExchangeRateResponse? _exchangeRateResponse;
    private IExchangeRateService _exchangeRateService;
    private ITelegramCalendarMarkup _telegramCalendarMarkup;
    private IMessageService _messageService;

    public HryvnianRateBotInstruction(IExchangeRateService exchangeRateService, ITelegramCalendarMarkup calendar, ILogger<HryvnianRateBotInstruction> logger,
        IMessageService messageService, IProvider provider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _exchangeRateService = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
        _telegramCalendarMarkup = calendar ?? throw new ArgumentNullException(nameof(calendar));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _bankProvider = provider ?? throw new ArgumentNullException(nameof(provider));

        _userState = new UserState();
        _currentCts = new CancellationTokenSource();
        _userLastMessageId = new Dictionary<long, int?>();
    }

    public async Task GetResponseToStartCommand(Message msg)
    {
        if (_userState.Id == 0)
            _userState.Id = msg.Chat.Id;

        await _messageService.SendMessage(msg.Chat.Id, BotMessage.WelcomeMessage, replyMarkup: InlineKeyboardMarkupExtension.GetMarkupForCurrencyCode());
    }

    public async Task GetResponseToNewOperationCommand(Message msg)
    {
        _currentCts.Cancel();
        _userState = new UserState { Id = msg.From!.Id };
        _userLastMessageId = new Dictionary<long, int?>();
        await _messageService.SendMessage(msg.Chat.Id, BotMessage.StartNewOperation);
        await SendNextStepMessage(msg.Chat.Id);
    }

    public async Task HandleCallbackQueryAsync(CallbackQuery query)
    {
        if (query.Data == "calendar" || query.Data!.StartsWith("year") || query.Data.StartsWith("month") || query.Data.StartsWith("day"))
        {
            _currentCts.Cancel();
            await GetInstructionForCalendarHandler_CallbackQueryAsync(query);
        }
        else if (Enum.TryParse(query.Data, true, out CurrencyCode currency))
        {
            await GetInstructionForCurrencyAccept(query, currency);
        }
        else if (DateOnly.TryParse(query.Data, out var parsedDate))
        {
            await GetInstructionForDateAccept(query, parsedDate);
        }
        else
            await _messageService.AnswerCallbackQuery(query.Id, BotMessage.InvalidOption);
    }

    private async Task GetInstructionForCalendarHandler_CallbackQueryAsync(CallbackQuery query)
    {
        var selectedDate = await _telegramCalendarMarkup.HandleCalendarCallbackQuery(query);
        if (selectedDate != null && DateOnly.TryParse(selectedDate, out var parsedDate))
            await GetInstructionForDateAccept(query, parsedDate);
    }

    private async Task GetInstructionForCurrencyAccept(CallbackQuery query, CurrencyCode currency)
    {
        _currentCts.Cancel();
        await _messageService.AnswerCallbackQuery(query.Id, string.Format(BotMessage.PickedCurrencyText, query.Data));
        _userState.Currency = currency;
        await SendNextStepMessage(query.From.Id);
    }

    private async Task GetInstructionForDateAccept(CallbackQuery query, DateOnly date)
    {
        _userState.Date = date;

        if (date == DateOnly.FromDateTime(DateTime.Now))
            await _messageService.SendOrEditMessageAsync(query.Message!.Chat.Id, query.Message.MessageId,
                string.Format(BotMessage.PickDate, date), replyMarkup: InlineKeyboardMarkupExtension.GetInlineMakrupForCallingOnlyCalendar());

        if (_userState.Currency != default)
            _exchangeRateResponse = await _bankProvider.GetExchangeRatesAsync(_userState.Date);
        await SendNextStepMessage(query.From.Id);
    }

    private async Task SendNextStepMessage(long chatId)
    {
        if (_userState.Currency == default)
        {
            await _messageService.SendMessage(chatId, BotMessage.ChooseCurrency,
                    replyMarkup: InlineKeyboardMarkupExtension.GetMarkupForCurrencyCode());
        }
        else if (_userState.Date == default)
        {
            await _messageService.SendMessage(chatId, string.Format(BotMessage.EnterDate, _userState.Currency),
                    replyMarkup: InlineKeyboardMarkupExtension.GetInlineMarkupForDateChoosing());
        }
        else
            await GetInstructionForFinalOperation_CallbackQueryAsync(chatId);
    }

    private async Task GetInstructionForFinalOperation_CallbackQueryAsync(long chatId)
    {
        if (!_userLastMessageId.TryGetValue(chatId, out int? messageId))
            messageId = null;

        messageId = await _messageService.SendOrEditMessageAsync(chatId, messageId, BotMessage.WaitMessageText);
        try
        {
            messageId = await GetResultOfRateOperation(chatId, messageId);
        }
        catch (EmptyBankInfoException exception)
        {
            _logger.LogInformation($"Send message for exception {exception.Message} for id {chatId}");
            messageId = await _messageService.SendOrEditMessageAsync(chatId, messageId,
                string.Format(BotMessage.NoBankInfo, _userState.Currency, _userState.Date));
        }
        catch (ArgumentNullException exception)
        {
            _logger.LogInformation($"{exception.Message} for id {chatId}");
            messageId = await _messageService.SendOrEditMessageAsync(chatId, messageId, BotMessage.NullMessageError);
        }
        catch (ApiRequestException exception)
        {
            _logger.LogError(exception.ToString());
            messageId = await _messageService.SendOrEditMessageAsync(chatId, messageId, BotMessage.ApiRequestException);
        }
        _userLastMessageId[chatId] = messageId;
    }

    private async Task<int> GetResultOfRateOperation(long chatId, int? messageId)
    {
        var result = _exchangeRateService.GetExchangeRateForClientsCurrency(_exchangeRateResponse!, _userState.Currency!)
            ?? throw new EmptyBankInfoException();

        string messageText = string.Format(BotMessage.ExchangeRateInfo, result.Currency,
            result.SaleRate.ToString("0.00"), result.PurchaseRate.ToString("0.00"),
                _userState.Date, BotMessage.BankName);

        if (result.SaleRate == 0)
            messageText = _exchangeRateService.ChangeResultsForNBUInfromation(result, _userState);

        return await _messageService.SendOrEditMessageAsync(chatId, messageId, messageText);
    }
}
