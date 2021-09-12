using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.CryptoTracker.Bot.Services.Commands
{
    public abstract class Command
    {

        public abstract string Name { get; }

        public abstract Task Execute(Update update, IBotService client);

        public abstract bool Contains(Message message);

        public abstract void Oncallback(Update update, IBotService client);


    }
}
