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
        
        [HttpPost("SellAllItems")]
        public async Task<IActionResult> SellAllItems(string sessionId, string steamLoginSecure, InputFilter filter)
        {
            try
            {
                if (!Enum.IsDefined(typeof(Category), filter.Category))
                    return BadRequest("Invalid category. Please select a valid category.");
                
                await _clientUseCases.SellAllItems(sessionId, steamLoginSecure, filter);
                return Ok();
            } catch (Exception ex)
            {
                //Add future log exception
                return StatusCode(500, ex.Message);
            }
        }
    }
}
