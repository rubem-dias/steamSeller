using Newtonsoft.Json;

namespace SteamItemSeller.Services.Dtos
{
    public class ItemPriceOverviewResponse
    {
        [JsonProperty("lowest_price")]
        public string? LowestPrice { get; set; }
        [JsonProperty("median_price")]
        public string? MedianPrice { get; set; }
        [JsonProperty("success")]
        public string? Success { get; set; }
        [JsonProperty("volume")]
        public string? Volume { get; set; }
    }
}
