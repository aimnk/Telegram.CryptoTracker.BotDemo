using System.Threading.Tasks;

namespace Telegram.CryptoTracker.Bot.Services.Commands.Tools
{
    public abstract class TokenInfoApi
    {

        public abstract Task<double> GetPriceTokenFromApi();


        public async Task<double> GetPriceToken()
        {
           return await GetPriceTokenFromApi();
        }
    }
}
