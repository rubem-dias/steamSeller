using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SteamItemSeller.Application.Dto;

public class InputFilter
{
    public Category Category { get; set; } = Category.None;
}

public enum Category
{
    [Display(Name = "None", Description = "None")]
    None,
    [Display(Name = "Card Border", Description = "Trading Card")]
    TradingCard,
}