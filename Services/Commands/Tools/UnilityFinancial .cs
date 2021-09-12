namespace Telegram.CryptoTracker.Bot.Services.Commands.Tools
{
    public class UtilityFinancial
    {

        public decimal GetProfit(decimal volume, decimal averagePurchasePrice, decimal priceTokenNow)
        {
            decimal priceToken = GetPriceToken(volume, averagePurchasePrice, priceTokenNow);
            return priceToken - volume;
        }


        public decimal GetPriceToken(decimal volume, decimal averagePurchasePrice, decimal priceTokenNow)
        {
            decimal amount = GetAmountToken(volume, averagePurchasePrice);
            return amount * priceTokenNow;
        }

        public decimal GetAmountToken(decimal volume, decimal averagePurchasePrice)
        {
            decimal result = volume / averagePurchasePrice;
            return result;
        }

        public (decimal finalPriceAverage, decimal finalVolume) GetPriceAverage(decimal startVolume, decimal endVolume, decimal startAveragePurchasePrice, decimal endAveragePurchasePrice)
        {
            decimal amountTokenStart = GetAmountToken(startVolume, startAveragePurchasePrice);
            decimal amountTokenEnd = GetAmountToken(endVolume, endAveragePurchasePrice);

            decimal finalAveragePurchasePrice = (amountTokenStart * startAveragePurchasePrice + amountTokenEnd * endAveragePurchasePrice) / (amountTokenStart + amountTokenEnd);
            decimal finalVolume = startVolume + endVolume;

            return (finalAveragePurchasePrice, finalVolume);
        }


    }
}
