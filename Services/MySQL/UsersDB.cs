using System;

namespace Telegram.CryptoTracker.Bot.Services.Mysql
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public long Chat_id { get; set; }
        public DateTime Date_reg { get; set; }
        public DateTime Date_expires { get; set; }

        public string Used_promo { get; set; }

        public DateTime Date_used_promo { get; set; }
    }
}
