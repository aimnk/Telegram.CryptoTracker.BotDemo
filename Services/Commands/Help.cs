using NLog;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace Telegram.CryptoTracker.Bot.Services.Commands
{
    public class Help : Command
    {
        public override string Name => @"/help";
        private readonly IBotService _botService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Help(IBotService botService)
        {
            _botService = botService;
          
        }
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async void Oncallback(Update update, IBotService botClient)
        {
           
            if (update.CallbackQuery.Data == "/price")
            {
                await _botService.Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)}Чтобы посмотреть текущую стоимость монеты \r\n" +
                        $"Введите {update.CallbackQuery.Data} ВАЛЮТНАЯПАРА \r\n" +
                        $"пример {update.CallbackQuery.Data} BTCUSDT \r\n");
            }
            if (update.CallbackQuery.Data == "/add")
            {
                await _botService.Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)}Для того чтобы добавить монету в ваш портфель\r\n" +
                        $"введите {update.CallbackQuery.Data} ВалютнаяПара СредняяСуммаПокупки ОбъемПокупки \r\n" +
                        $"Пример: /add LTCUSDT 201.8 150.5 \r\n(разделитель дробного числа точка)");
            }
            if (update.CallbackQuery.Data == "/delete")
            {
                await _botService.Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)}Для того чтобы удалить токен из вашего портфеля\r\n" +
                           $"введите {update.CallbackQuery.Data} НомерМонеты - номер монеты указан в вашем портфеле\r\n" +
                           $"или введите {update.CallbackQuery.Data} all - для полной очистки вашего портфеля");
            }
            if (update.CallbackQuery.Data == "/getportfolio")
            {
                GetPortfolio _getportfolio = new GetPortfolio(_botService);
                await _getportfolio.Execute(update, botClient);
            }
            
            if (update.CallbackQuery.Data == "/promo")
            {
                await _botService.Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)}Для активации промокода введите \r\n{update.CallbackQuery.Data} ПРОМОКОД");
            }
        }
        public override async Task Execute(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                  {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F503)}Показать листинг новых токенов", "/listing"),
                    },
                     new []
                    {
                        InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F525)}Показать даты сжигания монет", "/burning"),
                    },
                     new []
                    {
                        InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F4B2)}Показать текущую цену валютной пары", "/price"),
                    },
                      new []
                    {
                        InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F536)}Добавить монету в портфель", "/add"),
                    },
                       new []
                    {
                        InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x0274C)}Удалить монету из портфеля", "/delete"),
                    },
                       new []
                    {
                        InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F4BC)}Показать портфель", "/getportfolio"),
                    },
                 //      new []
                 //   {
                 //       InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F4B3)}Информация о подписке", "/sub"),
                 //   },
                 //        new []
                 //   {
                 //      InlineKeyboardButton.WithCallbackData($"{char.ConvertFromUtf32(0x1F192)}Ввод промокода", "/promo"),
                //    },
                });

            await botClient.Client.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"{char.ConvertFromUtf32(0x1F4AC)}Доступные команды",
                   replyMarkup: inlineKeyboard);

            _logger.Trace($"Command execution 'Help' from {message.Chat.Id}");
        }
    }
}
