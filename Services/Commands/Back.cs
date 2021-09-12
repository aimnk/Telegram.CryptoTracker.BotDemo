using NLog;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.CryptoTracker.Bot.Services.Commands
{
    public class Back : Command
    {
        public override string Name => @"/back";
        private readonly IBotService _botService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Back(IBotService botService)
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
            if (update.CallbackQuery.Data == "/back")
            {
                Help _help = new Help(_botService);
                await _help.Execute(update, botClient);
            }
           
        }
        public override async Task Execute(Update update, IBotService botClient)
        {
            var message = update.Message != null ? update.Message : update.CallbackQuery.Message;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                 {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Назад", "/back"),
                    }
                });
            await botClient.Client.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"{char.ConvertFromUtf32(0x025C0)}Вернуться назад к списку команд?",
                   replyMarkup: inlineKeyboard);

            _logger.Trace($"Command execution 'back' from {message.Chat.Id}");
        }
    }
}