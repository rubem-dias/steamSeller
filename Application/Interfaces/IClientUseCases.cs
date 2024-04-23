using Microsoft.AspNetCore.Mvc;
using SteamItemSeller.Application.Dto;
using SteamItemSeller.Services.Dtos;

namespace SteamItemSeller.Application.Interfaces
{
    public interface IClientUseCases
    {
        public Task<List<ItemPostOrder>> SellItems(string sessionId, string steamLoginSecure, InputFilter? filter);
    }
}
