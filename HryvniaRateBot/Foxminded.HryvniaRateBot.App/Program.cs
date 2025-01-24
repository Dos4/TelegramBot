using Foxminded.HryvniaRateBot.App.DataAccess;
using Foxminded.HryvniaRateBot.App.DataAccess.Providers;
using Foxminded.HryvniaRateBot.App.Services;
using Foxminded.HryvniaRateBot.App.TelegramBot;
using Foxminded.HryvniaRateBot.App.TelegramBot.Markups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

namespace Foxminded.HryvniaRateBot.App;

class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.Services.GetRequiredService<HryvnianRateBot>();

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((context, config) =>
             {
                 var environment = context.HostingEnvironment.EnvironmentName;
                 config.SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json")
                       .AddJsonFile($"appsettings.{environment}.json")
                       .AddEnvironmentVariables();
             })
             .ConfigureLogging((context, logging) =>
             {
                 logging.AddSerilog(new LoggerConfiguration()
                     .ReadFrom.Configuration(context.Configuration)
                     .CreateLogger());
             })
             .ConfigureServices((context, services) =>
             {
                 services.AddHttpClient();

                 var botToken = context.Configuration["Telegram:BotToken"];
                 services.AddSingleton(new TelegramBotClient(botToken!));

                 services
                     .Configure<CurrencyProviderOptions>(context.Configuration.GetSection("CurrencyProviders"))
                     .Configure<PrivatBankOptions>(context.Configuration.GetSection("CurrencyProviders:PrivatBank"))
                     .AddTransient<ITelegramCalendarMarkup, TelegramCalendarMarkup>()
                     .AddSingleton<HryvnianRateBotInstruction>()
                     .AddTransient<HryvnianRateBot>()
                     .AddTransient<PrivatBankProvider>()
                     .AddTransient<IProvider>(provider =>
                     {
                         var options = provider.GetRequiredService<IOptions<CurrencyProviderOptions>>().Value;
                         return options.ActiveProvider switch
                         {
                             "PrivatBank" => provider.GetRequiredService<PrivatBankProvider>(),
                             _ => throw new InvalidOperationException()
                         };
                     })
                     .AddTransient<IExchangeRateService, ExchangeRateService>()
                     .AddTransient<IMessageService, MessageService>();
             });
    }
}