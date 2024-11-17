using Microsoft.AspNetCore.Mvc;
using SteamItemSeller.Application.Dto;
using SteamItemSeller.Services.ApiServices;
using SteamItemSeller.Services.Dtos;

namespace SteamItemSeller.Application.Interfaces
{
    public interface IClientUseCases
    {
        public Task<List<ItemPostOrder>> SellItems(string sessionId, string steamLoginSecure, string userProfile, InputFilter? filter);
    }
}
