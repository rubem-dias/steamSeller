using Newtonsoft.Json;

namespace SteamItemSeller.Services.Dtos
{
    public class InventoryResponse
    {
        public List<Asset> Assets { get; set; } = [];
        public List<Description> Descriptions { get; set; } = [];
    }
    public class Asset
    {
        public int AppId { get; set; }
        public string? ContextId { get; set; }
        public string? AssetId { get; set; }
        public string? ClassId { get; set; }
        public string? InstanceId { get; set; }
        public string? Amount { get; set; }
    }
    public class Description
    {
        public int AppId { get; set; }
        public string? ClassId { get; set; }
        public string? InstanceId { get; set; }
        public int Tradable { get; set; }
        public string? Name { get; set; }
        [JsonProperty("market_name")]
        public string? MarketName { get; set; }
        [JsonProperty("market_hash_name")]
        public string? MarketHashName { get; set; }
        public List<Tag> Tags { get; set; } = [];
    }
    
    public class Tag
    {
        public string? Category { get; set; }
        [JsonProperty("internal_name")]
        public string? InternalName { get; set; }
        public string? LocalizedCategoryName { get; set; }
        public string? LocalizedTagName { get; set; }
    }
}
