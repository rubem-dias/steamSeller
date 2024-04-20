using Microsoft.AspNetCore.Mvc;
using SteamItemSeller.Application.Interfaces;
using System.Web;

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
        
        [HttpGet("SellAllItems")]
        public async Task<IActionResult> SellAllItems(string sessionId, string steamLoginSecure)
        {
            try
            {
                await _clientUseCases.SellAllItems(sessionId, steamLoginSecure);
                return Ok();
            } catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar o login.", ex.InnerException);
            }
        }
    }
}
