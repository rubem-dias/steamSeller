using Microsoft.AspNetCore.Mvc;
using SteamItemSeller.Application.Dtos;

namespace SteamItemSeller.Application.Interfaces
{
    public interface IClientUseCases
    {
        public Task SellAllItems(string sessionId, string steamLoginSecure);
    }
}
