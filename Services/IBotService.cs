using Telegram.Bot;

namespace Telegram.CryptoTracker.Bot.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }

    }
}