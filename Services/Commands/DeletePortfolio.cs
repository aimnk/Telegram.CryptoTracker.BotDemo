using NLog;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.CryptoTracker.Bot.Services.MySQL;

namespace Telegram.CryptoTracker.Bot.Services.Commands
{
    public class DeletePortfolio : Command
    {
        public override string Name => @"/delete";
        private readonly IBotService _botService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public DeletePortfolio(IBotService botService)
        {
            _botService = botService;
        }
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override void Oncallback(Update update, IBotService botClient)
        {

        }
        public override async Task Execute(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            if (update.Message.Text == "/delete")
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)}" +
                    $"Для того чтобы удалить токен из вашего портфеля\r\n" +
                    "введите /delete НомерМонеты - номер монеты указан в вашем портфеле\r\n" +
                    "или введите /delete all - для полной очистки вашего портфеля");

            var utilityMуSQL = new UtilityMySQL();
            var userData = utilityMуSQL.GetData(message.Chat.Id);

            if (userData == null)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, $"{char.ConvertFromUtf32(0x0274C)}Вам нечего удалять");
                Back _back = new Back(_botService);
                await _back.Execute(update, botClient);
            }

            if (update.Message.Text == "/delete all")
            {
                 removeFullPortfolio(utilityMуSQL, message);
                _logger.Trace($"Command execution 'delete all porfolio' from {message.Chat.Id}");

                Back _back = new Back(_botService);
                await _back.Execute(update, botClient);
            }
            else
            {
                 removeTokenFromPortfolio(utilityMуSQL, message);
                _logger.Trace($"Command execution 'delete token' from {message.Chat.Id}");

                Back _back = new Back(_botService);
                await _back.Execute(update, botClient);
            }
 
        }


        private async void removeFullPortfolio (UtilityMySQL utilityMSQL, Message message)
        {
                utilityMSQL.DeleteDataFull(message.Chat.Id);

                await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x2705)}Очистка портфеля прошла успешно");

        }

        private async void removeTokenFromPortfolio(UtilityMySQL utilityMSQL, Message message)
        {

            if (Int32.TryParse(message.Text.Split()[1], NumberStyles.Float, 
                                      CultureInfo.InvariantCulture, out int rowID))
            {
                (bool result, string tokenName) resultDelete = utilityMSQL.DeleteData(message.Chat.Id, rowID);

                if (resultDelete.result == true)
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x2705)}" +
                        $"Монета {resultDelete.tokenName} удалена из вашего портфеля");
                else
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x0274C)}" +
                        $"Не найдены монеты для удаления");
            }
        }
    }
}
