namespace SteamItemSeller.Services.Dtos
{
    public class ItemToSell
    {
        public required string SessionId { get; set; }
        public required string AppId { get; set; }
        public required string ContextId { get; set; }
        public required string AssetId { get; set; }
        public required string Amount { get; set; }
        public required string Price { get; set; }

        //Not Required To Post
        public string? Name { get; set; }
    }
}
