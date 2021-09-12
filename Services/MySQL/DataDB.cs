namespace Telegram.CryptoTracker.Bot.Services.Mysql
{
    public class Data
    {

        public int Id { get; set; }
        public long Chat_id { get; set; }
        public string Token { get; set; }

        public decimal Volume { get; set; }

        public decimal PriceAverage { get; set; }

    }
}
