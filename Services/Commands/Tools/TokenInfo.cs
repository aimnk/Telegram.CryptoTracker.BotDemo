using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telegram.CryptoTracker.Bot.Services.Commands.Tools
{

    public class TokenInfo
    {
        public async virtual Task<double> GetPriceToken(string symbolToken)
        {           
            var tokenInfoBinance = new TokenInfoBinance(symbolToken);
            double priceToken = await tokenInfoBinance.GetPriceToken();

            if (priceToken == 0)
                return 0;
            else
                return priceToken;

        }

        public async Task<bool> ExistSymbolToken(string symbolToken)

        {
            var tokenInfCoinBinance = new TokenInfoBinance(symbolToken);
            var existsTokenSymbol = await tokenInfCoinBinance.CheckTokenName();

            if (existsTokenSymbol == false)
                return existsTokenSymbol;
            else
                return existsTokenSymbol;
        }    
    }
}
