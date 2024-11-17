using Microsoft.AspNetCore.Mvc;
using SteamItemSeller.Application.Interfaces;
using System.Web;
using SteamItemSeller.Application.Dto;

namespace SteamItemSeller.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {

        private readonly IClientUseCases _clientUseCases;

        public ClientController(IClientUseCases clientUseCases)
        { 
            _clientUseCases = clientUseCases;
        }
        
        [HttpPost("SellItems")]
        public async Task<IActionResult> SellItems(string sessionId, string steamLoginSecure, string userProfile, InputFilter? filter)
        {
            try
            {
                if (!Enum.IsDefined(typeof(Category), filter.Category))
                    return BadRequest("Invalid category. Please select a valid category.");
                
                var response = await _clientUseCases.SellItems(sessionId, steamLoginSecure, userProfile, filter);
                return Ok(response);

            } catch (Exception ex)
            {
                //Add future log exception
                return StatusCode(500, ex.Message);
            }
        }
    }
}
