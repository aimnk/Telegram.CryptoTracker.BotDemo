using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.CryptoTracker.Bot.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
