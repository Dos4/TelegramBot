using Microsoft.Extensions.Logging;
using System.Globalization;
using Telegram.Bot.Types;

namespace Foxminded.HryvniaRateBot.App.Resources;

public static class LocalizationManager
{
    public static void SetCulture(string languageCode)
    {
        var culture = new CultureInfo(languageCode);
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    public static void SetLanguageForUser(User user, ILogger logger)
    {
        var languageCode = user.LanguageCode ?? "en";
        try
        {
            SetCulture(languageCode);
        }
        catch (CultureNotFoundException exception)
        {
            logger.LogInformation($"user language code is: {languageCode}. System thrown the exception {exception.Message}");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
        }
    }
}
