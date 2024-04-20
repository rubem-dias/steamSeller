using Newtonsoft.Json;

namespace SteamItemSeller.Application.Dtos
{
    public class InventoryDto
    {
        public List<Asset> Assets { get; set; }
        public List<Description> Descriptions { get; set; }
    }

    public class Asset
    {
        public int AppId { get; set; }
        public string ContextId { get; set; }
        public string AssetId { get; set; }
        public string ClassId { get; set; }
        public string InstanceId { get; set; }
        public string Amount { get; set; }
    }

    public class Tag
    {
        public string Category { get; set; }
        public string InternalName { get; set; }
        public string LocalizedCategoryName { get; set; }
        public string LocalizedTagName { get; set; }
    }

    public class Description
    {
        public int AppId { get; set; }
        public string ClassId { get; set; }
        public string InstanceId { get; set; }
        public int Currency { get; set; }
        public string BackgroundColor { get; set; }
        public string IconUrl { get; set; }
        public string IconUrlLarge { get; set; }
        [JsonProperty("descriptions")]
        public List<DescriptionDetail> DescriptionDetails { get; set; }
        public int Tradable { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string MarketName { get; set; }
        public string MarketHashName { get; set; }
        public int MarketFeeApp { get; set; }
        public int Commodity { get; set; }
        public int MarketTradableRestriction { get; set; }
        public int MarketMarketableRestriction { get; set; }
        public int Marketable { get; set; }
        public List<Tag> Tags { get; set; }
    }

    public class DescriptionDetail
    {
        public string Value { get; set; }
    }

}
