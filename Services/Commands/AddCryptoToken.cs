using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.CryptoTracker.Bot.Services.Commands.Tools;
using Telegram.CryptoTracker.Bot.Services.Mysql;
using Telegram.CryptoTracker.Bot.Services.MySQL;

namespace Telegram.CryptoTracker.Bot.Services.Commands
{
    public class AddCryptoToken : Command
    {
        public override string Name => @"/add";
        private readonly IBotService _botService;
        private static Message _messageStart;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public AddCryptoToken(IBotService botService)
        {
            _botService = botService;
        }

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            else
            {
                _messageStart = message;
                return message.Text.Contains(this.Name);
            }
        }
        public override void Oncallback(Update update, IBotService botClient)
        {
            if (update.CallbackQuery.Data == "summyes" & _messageStart != null)
                repeatTokenAndSendMessage(update, botClient);


            if (update.CallbackQuery.Data == "summno" & _messageStart != null)
                addTokenAndSendMessage(update, botClient);
        }

       
        public override async Task Execute(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;


            if (message.Text == "/add")
                await botClient.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)}Для того чтобы добавить монету в ваш портфель\r\n" +
                    "введите /add ВалютнаяПара СредняяСуммаПокупки ОбъемПокупки \r\n");
            else
                repeatOrAddToken(update, botClient);

            _logger.Trace($"Command execution 'addToken' from {message.Chat.Id}");
        }


        private async void repeatOrAddToken(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            var utilityMySQL = new UtilityMySQL();

            if (await checkExistsUserOrSendMessage(update, utilityMySQL, botClient))
            {
                List<Data> userData = utilityMySQL.GetData(message.Chat.Id);

                if (await checkValidationMessageOrSendMessage(update, botClient))
                {
                    var messageValidation = await getTokenPriceFromMessage(_messageStart);
                    string symbolToken = messageValidation.symbolToken;

                    Data existsTokenPortfolio = userData.FirstOrDefault(p => p.Token == symbolToken);

                    if (existsTokenPortfolio == null)
                        addTokenAndSendMessage(update, botClient);
                    else
                        await tokenAveraging(update, botClient);
                }
            }

        }

        private async void repeatTokenAndSendMessage(Update update, IBotService botClient)
        {
            var message = _messageStart;

            var utilityMySQL = new UtilityMySQL();
            var utilityFinancial = new UtilityFinancial();

            string symbolToken = message.Text.Split()[1].ToUpper();
            Data userData = utilityMySQL.FindRepeatingToken(message.Chat.Id, symbolToken);
            var tokenPrice = await getTokenPriceFromMessage(message);

            var finalPriceToken = utilityFinancial.GetPriceAverage(userData.Volume, tokenPrice.volume, userData.PriceAverage, tokenPrice.averagePurchasePrice);

            utilityMySQL.RepeatData(message.Chat.Id, symbolToken, finalPriceToken.finalPriceAverage, finalPriceToken.finalVolume);

            await botClient.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x2705)}Усреднение монеты прошло пуспешно");


            Back _back = new Back(_botService);
            await _back.Execute(update, botClient);

            _logger.Trace($"Command execution 'RepeatToken' successfully from {message.Chat.Id}");
        }


        private async void addTokenAndSendMessage (Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            var utilityMySQL = new UtilityMySQL();

            var tokenPrice = await getTokenPriceFromMessage(_messageStart);

            utilityMySQL.AddData(message.Chat.Id, tokenPrice.symbolToken, tokenPrice.averagePurchasePrice, tokenPrice.volume);

           await botClient.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x2705)}Монета успешно добавлена в портфель ");


            Back _back = new Back(_botService);
            await _back.Execute(update, botClient);

            _logger.Trace($"Command execution 'AddToken' successfully from {message.Chat.Id}");
        }


        private async Task tokenAveraging(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
               {
                                  new []
                                 {
                                  InlineKeyboardButton.WithCallbackData("Да", "summyes"),
                                  InlineKeyboardButton.WithCallbackData("Нет", "summno"),
                                  }
                        });

            await botClient.Client.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"{char.ConvertFromUtf32(0x1F4A1)}У вас уже есть такая монета в портфеле, хотите усреднить?",
                   replyMarkup: inlineKeyboard);
        }

        private async Task<(string symbolToken, decimal averagePurchasePrice, decimal volume)> getTokenPriceFromMessage(Message message)
        {
            var tokenInfo = new TokenInfo();

            string symbolToken = message.Text.Split()[1].ToUpper();

            if (message.Text.Split().Length == 4 && await tokenInfo.ExistSymbolToken(symbolToken))
            {

                if (Decimal.TryParse(message.Text.Split()[2], NumberStyles.Float,
                                      CultureInfo.InvariantCulture, out decimal averagePurchasePrice) &
                                    Decimal.TryParse(message.Text.Split()[3], NumberStyles.Float,
                                       CultureInfo.InvariantCulture, out decimal volume))
                    return (symbolToken, averagePurchasePrice, volume);
            }

            return (string.Empty, 0, 0);
        }

        private async Task<bool> checkExistsUserOrSendMessage(Update update, UtilityMySQL UtilityMySQL, IBotService botClient)
        {

            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            if (UtilityMySQL.ExistsUser(message.Chat.Id) == null)
            {
                await botClient.Client.SendTextMessageAsync(message.Chat.Id, $"Сначала необходимо зарегистрироваться \r\n для регистрации введите команду /reg");
                return false;
            }
            return true;
        }

        private async Task<bool> checkValidationMessageOrSendMessage(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            var messageValidation = await getTokenPriceFromMessage(_messageStart);
            if (string.IsNullOrEmpty(messageValidation.symbolToken))
            {
                await botClient.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x0274C)}" +
                    $"Произошла ошибка, проверьте корректность ввода \r\n" +
                    $"Пример: /add BTCUSDT 58000.5 0.1 \r\n(разделитель дробного числа точка)");
                return false;
            }
            return true;
        }

    }
}
