using NLog;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.CryptoTracker.Bot.Services.Commands.Tools;

namespace Telegram.CryptoTracker.Bot.Services.Commands
{


    public class GetPriceCrypto : Command
    {
        public override string Name => @"/price";
        private readonly IBotService _botService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GetPriceCrypto(IBotService botService)
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

            if (message.Text == "/price")
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x2757)} " +
                    $"Чтобы посмотреть текущую стоимость монеты \r\n" +
                    $"Введите /price ВАЛЮТНАЯПАРА \r\n" +
                    $"пример /price BTCUSDT \r\n");
            else
            {
                    var tokenInfo = new TokenInfo();

                    string sumbolToken = message.Text.Split()[1].ToUpper();

                    decimal priceToken = await tokenInfo.GetPriceToken(sumbolToken);

                    if (priceToken > 0)

                    {

                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x1F536)} " +
                                                                   $"Стоимость " + $"{sumbolToken} : {priceToken}");

                        logger.Trace($"Command execution 'GetPriceCrypto' successfully from {message.Chat.Id}");
                        Back _back = new Back(_botService);
                        await _back.Execute(update, botClient);


                    }

                    else
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x0274C)}" +
                            $"Не удалось определить стоимость монеты, возможно вы указали монету неверно");
                        logger.Error($"Command execution 'GetPriceCrypto' failed from {message.Chat.Id} Exception: Token cost is null");
                    }
               
            }

        }
    }
}
