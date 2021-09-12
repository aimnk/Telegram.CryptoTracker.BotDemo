
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Telegram.CryptoTracker.Bot.Services.Mysql
{


    public class ApplicationContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Data> Data { get; set; }
        public DbSet<Donation> Donation { get; set; }

        public DbSet<Promocodes> Promocodes { get; set; }


        public ApplicationContext()

        {

            Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
             .AddJsonFile("appsettings.json")
              .Build();

            optionsBuilder.UseMySql(builder.GetConnectionString("DefaultConnection"));

        }

    }
}
