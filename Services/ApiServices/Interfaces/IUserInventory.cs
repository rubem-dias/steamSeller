using Microsoft.AspNetCore.Components.Forms;
using SteamItemSeller.Application.Dto;
using SteamItemSeller.Services.Dtos;

namespace SteamItemSeller.Services.SteamServices.Interfaces;

public interface IUserInventory
{
    public Task<List<ItemPostOrder?>> OrderedItemsToSell(UserData userData, InputFilter filter, string sessionId);
}