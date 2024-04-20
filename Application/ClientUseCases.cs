using SteamItemSeller.Application.Interfaces;
using SteamItemSeller.Services.SteamServices.Interfaces;

namespace SteamItemSeller.Application
{
    public class ClientUseCases : IClientUseCases
    {
        private readonly IAuthentication _authentication;

        public ClientUseCases(IAuthentication authentication)
        {
            _authentication = authentication;
        }
        public async Task SellAllItems(string sessionId, string steamLoginSecure)
        {
            var userId = await _authentication.GetUserId(sessionId, steamLoginSecure);
        }
    }
}
