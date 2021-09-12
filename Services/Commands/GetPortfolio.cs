using NLog;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.CryptoTracker.Bot.Services.Commands.Tools;
using Telegram.CryptoTracker.Bot.Services.Mysql;
using Telegram.CryptoTracker.Bot.Services.MySQL;

namespace Telegram.CryptoTracker.Bot.Services.Commands



{
    public class GetPortfolio : Command
    {
        public override string Name => @"/getportfolio";
        private readonly IBotService _botService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GetPortfolio(IBotService botService)
        {
            _botService = botService;
        }

        class CryptoToken
        {
           
            public string price { get; set; }
            public string symbol { get; set; }
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

            var utilityMуSQL = new UtilityMySQL();
            var userData = utilityMуSQL.GetData(message.Chat.Id);
          

                if (userData != null)
                    {                    
                        int i = 1;
                        double summvolume = 0;
                        double summprofit = 0;
                        var tokenInfo = new TokenInfo();

                     foreach (Data u in userData)
                          {


                            double priceToken = await tokenInfo.GetPriceToken(u.Token);
                            double amount = u.Volume / u.PriceAverage;
                            double fullprice = amount * priceToken;
                            double profit = fullprice - u.Volume;

                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x0023)} {i}\r\n" +
                                $"{char.ConvertFromUtf32(0x1F536)} Монета: {u.Token}\r\n" +
                                $"Объем: {(Math.Round(u.Volume, 5))}$\r\n" +
                                $"Средняя цена покупки: {(Math.Round(u.PriceAverage, 5))}\r\n" +
                                $"Текущая цена: {priceToken}\r\n" +
                                $"Количество: {Math.Round(amount, 5)}\r\n" +
                                $"Стоимость: {Math.Round(fullprice, 5)}$\r\n" +
                                $"{ (profit >= 0 ? (char.ConvertFromUtf32(0x1F53C)) : (char.ConvertFromUtf32(0x1F53D)))} Прибыль: {Math.Round(profit, 5)}$");

                            summprofit += profit;
                            summvolume += u.Volume;
                            i++;                      
                        }

                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{char.ConvertFromUtf32(0x1F4BC)} " +
                            $"Цена портфеля начальная: {Math.Round(summvolume, 5)}$\r\n" +
                            $"{char.ConvertFromUtf32(0x1F4BC)} Цена портфеля текущая: {Math.Round(summvolume + summprofit, 5)}$\r\n" +
                            $"{ (summprofit >= 0 ? (char.ConvertFromUtf32(0x1F53C)) : (char.ConvertFromUtf32(0x1F53D)))} " +
                            $"Прибыль по портфелю: {Math.Round(summprofit, 5)}$");
                    }
                
            logger.Trace($"Command execution 'GetPortfolio' from {message.Chat.Id}");
            Back _back = new Back(_botService);
            await _back.Execute(update, botClient);
        }
    }
}
