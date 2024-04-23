using System.Text.RegularExpressions;

namespace SteamItemSeller.Util
{
    public static class CurrencyHandler
    {
        public static string ConvertPrice(this string price)
        {
            var steamFeePercent = 0.95; //steam fee affect in 5% in item

            string numericString = Regex.Replace(price, @"[^\d,]", "");
            string[] parts = numericString.Split(',');

            string integerPart = parts[0];
            string fractionalPart = parts.Length > 1 ? parts[1] : "0";

            if (fractionalPart.Length == 1)
                fractionalPart += "0"; // Append zero to make it two digits
            else if (fractionalPart.Length > 2)
                fractionalPart = fractionalPart.Substring(0, 2); // Truncate if more than two digits

            string totalCents = integerPart + fractionalPart;

            int cents = int.Parse(totalCents);

            int discountedPrice = (int)Math.Floor(cents * steamFeePercent);

            return discountedPrice.ToString(); // Convert the discounted price back to a string

        }
    }
}
