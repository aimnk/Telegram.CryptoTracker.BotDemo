namespace Telegram.CryptoTracker.Bot.Services.Commands.Tools
{
    public class UtilityFinancial
    {

        public double GetProfit(double volume, double averagePurchasePrice, double priceTokenNow)
        {
            double priceToken = GetPriceToken(volume, averagePurchasePrice, priceTokenNow);
            return priceToken - volume;
        }


        public double GetPriceToken(double volume, double averagePurchasePrice, double priceTokenNow)
        {
            double amount = GetAmountToken(volume, averagePurchasePrice);
            return amount * priceTokenNow;
        }

        public double GetAmountToken(double volume, double averagePurchasePrice)
        {
            double result = volume / averagePurchasePrice;
            return result;
        }

        public (double finalPriceAverage, double finalVolume) GetPriceAverage(double startVolume, double endVolume, double startAveragePurchasePrice, double endAveragePurchasePrice)
        {
            double amountTokenStart = GetAmountToken(startVolume, startAveragePurchasePrice);
            double amountTokenEnd = GetAmountToken(endVolume, endAveragePurchasePrice);

            double finalAveragePurchasePrice = (amountTokenStart * startAveragePurchasePrice + amountTokenEnd * endAveragePurchasePrice) / (amountTokenStart + amountTokenEnd);
            double finalVolume = startVolume + endVolume;

            return (finalAveragePurchasePrice, finalVolume);
        }


    }
}
