using NLog;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.CryptoTracker.Bot.Services.MySQL;

namespace Telegram.CryptoTracker.Bot.Services.Commands
{
    public class Start : Command
    {
        public override string Name => @"/start";
        private readonly IBotService _botService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();


        public Start(IBotService botService)
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

               var utilityMуSQL = new UtilityMySQL();
               utilityMуSQL.CreateUser(message.Chat.Id, message.Chat.Username);

               Help help = new Help(_botService);
               await help.Execute(update, botClient);

               _logger.Trace($"Command execution 'Start' from {message.Chat.Id}");
        }
    }
}