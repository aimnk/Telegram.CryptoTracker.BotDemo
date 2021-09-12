using Newtonsoft.Json;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Telegram.CryptoTracker.Bot.Services.Commands.Tools
{
    public class TokenInfoBinance : TokenInfoApi


    {

        public TokenInfoBinance(string symbolToken) => SymbolToken = symbolToken;

        private string _symbolToken;
        public string SymbolToken
        {
            get
            {
                return _symbolToken;
            }

            set
            {
                if (!String.IsNullOrEmpty(value) && !value.Contains("USDT"))
                    _symbolToken = value.Insert(value.Length, "USDT");
                else
                    _symbolToken = value;
            }
        }

        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private string _pageURL = $"https://www.binance.com/api/v3/ticker/price?symbol=";

        private class CryptoToken
        {
            public string symbol { get; set; }
            public decimal price { get; set; }
        }

        public async override Task<decimal> GetPriceTokenFromApi()
        {

                string bodyPage = await getPageApi();

                var infoToken = JsonConvert.DeserializeObject<CryptoToken>(bodyPage);

                if (infoToken == null)
                    return 0;

                if (infoToken.price < Convert.ToDecimal(0.0001))
                    return infoToken.price;
                else
                    return Math.Round(infoToken.price, 5);

        }

        public async Task<bool> CheckTokenName()
        {
            string bodyPage = await getPageApi();  
            if (bodyPage.Contains(_symbolToken))
                return true;
            else
                return false;
        }

        private async Task<string> getPageApi()
        {

            HttpClient client = new HttpClient();
            HttpResponseMessage responce = await client.GetAsync(_pageURL+_symbolToken);

            if (responce.StatusCode == HttpStatusCode.OK)
            {
                string body = await responce.Content.ReadAsStringAsync();

                return body;
            }

            else
            {
                _logger.Trace($"Error connection StatusCode: {responce.StatusCode}");
                client.Dispose();
                return string.Empty;
            }

         }

        
      
    }
}
