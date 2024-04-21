using Microsoft.AspNetCore.Components.Forms;
using SteamItemSeller.Application.Dto;
using SteamItemSeller.Services.Dtos;

namespace SteamItemSeller.Services.SteamServices.Interfaces;

public interface IUserInventory
{
    public Task<InventoryDto?> GetAllItems(string profileUri, InputFilter filter);
}