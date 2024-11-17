using SteamItemSeller.Application.Dto;
using SteamItemSeller.Application.Interfaces;
using SteamItemSeller.Services;
using SteamItemSeller.Services.Dtos;
using SteamItemSeller.Services.SteamServices.Interfaces;

namespace SteamItemSeller.Application
{
    public class ClientUseCases : IClientUseCases
    {
        private readonly IUserProfile _userProfile;
        private readonly IUserInventory _userInventory;
        public ClientUseCases(IUserProfile userProfile, IUserInventory userInventory)
        {
            _userProfile = userProfile;
            _userInventory = userInventory;
        }
        public async Task<List<ItemPostOrder>> SellItems(string sessionId, string steamLoginSecure, string userProfile, InputFilter? filter)
        {
            try
            {
                var userData = await _userProfile.GetUserProfileData(sessionId, steamLoginSecure, userProfile);
                var orderedItemsToSell = await _userInventory.OrderedItemsToSell(userData, filter!, sessionId);

                return orderedItemsToSell!;

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
