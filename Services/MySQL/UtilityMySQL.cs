using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.CryptoTracker.Bot.Services.Mysql;

namespace Telegram.CryptoTracker.Bot.Services.MySQL
{
    public class UtilityMySQL

    {

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public void CreateUser(long userID, string userNickName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {

                if (ExistsUser(userID) == null)
                {

                    Users newUser = new Users
                    {
                        Chat_id = userID,
                        Username = userNickName,
                        Date_reg = DateTime.Now,
                        Date_expires = DateTime.Now.AddDays(14)
                    };

                    db.Users.AddRange(newUser);
                    db.SaveChanges();
                    _logger.Info($"New user ID:{userID}");

                }

                else
                {
                    _logger.Trace($"User already register ID:{userID}");
                }

            }
        }
        public Users ExistsUser(long userID)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var existUserID = db.Users.FirstOrDefault(p => p.Chat_id == userID);

                if (existUserID != null)
                    return existUserID;
                else
                    return null;
            }
        }


        public List<Data> GetData(long userID)
        {
            using (ApplicationContext db = new ApplicationContext())
            {

                var result = db.Data.Where(p => p.Chat_id == userID);
                return result.ToList<Data>();
            }
        }

        public void DeleteDataFull(long userID)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var userData = GetData(userID);
                foreach (Data u in userData)
                {
                    db.Data.Remove(u);
                }

                db.SaveChanges();
            }
        }

        public (bool result, string nameToken) DeleteData(long userID, int rowID)
        {
            string nameToken = "";

            using (ApplicationContext db = new ApplicationContext())

            {
                var userData = GetData(userID);
                int i = 1;
                foreach (Data u in userData)
                {
                    if (i == rowID)
                    {
                        db.Data.Remove(u);
                        nameToken = u.Token;
                        db.SaveChanges();
                        return (true, nameToken);
                    }

                    i++;
                }

            }

            return (false, nameToken);
        }

        public void AddData(long userID, string symbolToken, double averagePurchasePrice, double volume)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Data data = new Data { Chat_id = userID, Token = symbolToken, PriceAverage = averagePurchasePrice, Volume = volume };
                db.Data.AddRange(data);
                db.SaveChanges();
            }
        }

        public void RepeatData(long userID, string symbolToken, double averagePurchasePrice, double volume)
        {

            var userData = GetData(userID);
            var foundData = userData.FirstOrDefault(p => p.Token == symbolToken);
            using (ApplicationContext db = new ApplicationContext())
            {
                 
                    foundData.Chat_id = userID;
                    foundData.Token = symbolToken;
                    foundData.PriceAverage = averagePurchasePrice;
                    foundData.Volume = volume;
                    db.Entry(foundData).State = EntityState.Modified;
                   db.SaveChanges();
            }
        }
  

        public Data FindRepeatingToken (long userID, string symbolToken)
        {
            var userData = GetData(userID);
            return userData.FirstOrDefault(p => p.Token == symbolToken);
        }
    }
}
