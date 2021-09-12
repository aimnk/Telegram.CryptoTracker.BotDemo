namespace Telegram.CryptoTracker.Bot.Services.Mysql
{
    public class Data
    {

        public int Id { get; set; }
        public long Chat_id { get; set; }
        public string Token { get; set; }

        public double Volume { get; set; }

        public double PriceAverage { get; set; }

    }
}
