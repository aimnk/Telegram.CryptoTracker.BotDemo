using System.Threading.Tasks;

namespace Telegram.CryptoTracker.Bot.Services.Commands.Tools
{
    public abstract class TokenInfoApi
    {

        public abstract Task<decimal> GetPriceTokenFromApi();


        public async Task<decimal> GetPriceToken()
        {
           return await GetPriceTokenFromApi();
        }
    }
}
