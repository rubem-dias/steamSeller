using SteamItemSeller.Application.Dto;
using SteamItemSeller.Application.Interfaces;
using SteamItemSeller.Services;
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
        public async Task SellAllItems(string sessionId, string steamLoginSecure, InputFilter? filter)
        {
            try
            {
                var userProfileUri = await _userProfile.GetUserProfileData(sessionId, steamLoginSecure);
                var getAllItems = await _userInventory.GetAllItems(userProfileUri.ProfileUrl!, filter!);

                //TODO API TO GET MEDIUM PRICES PER ITEM
                //FILTER FOR SELECT CATEGORIES ETC
                //FINALLY CALL SELL METHOD ITEMS

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
