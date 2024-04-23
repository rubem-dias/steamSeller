namespace SteamItemSeller.Services.Dtos
{
    public class UserWalletCredentials
    {
        public string SteamId { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        public string ContextId { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
