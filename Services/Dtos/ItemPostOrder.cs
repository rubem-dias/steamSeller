using System.ComponentModel.DataAnnotations;

namespace SteamItemSeller.Services.Dtos
{
    public class ItemPostOrder
    {
        public required string Name { get; set; }
        public required string Price { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Status Status { get; set; }
    }

    public enum Status
    {
        [Display(Name = "Item(s) successfully put up for sale")]
        Ok,
        [Display(Name = "Failed to put up for sale")]
        Failed
    }
}
