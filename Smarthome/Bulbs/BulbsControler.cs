
using Microsoft.AspNetCore.Mvc;
using Smarthome.Bulbs.interfaces;


namespace Smarthome.Bulbs
{
    [ApiController]
    [Route("[controller]")]
    public class BulbsController : ControllerBase
    {
        private readonly IBulbsService _bulbsService;

        public BulbsController(IBulbsService bulbsService) => _bulbsService = bulbsService;

        [HttpGet("{roomId?}")]
        public async Task<IActionResult> GetBulbs(int? roomId)
        {
            var bulbs = await _bulbsService.GetBulbsInfo(roomId);
            return Ok(bulbs);
        }


        [HttpPost("switch/{bulbId}")]
        public async Task<IActionResult> SwitchBulb([FromBody] IBulbSwitchRequest switchBulbDto, string bulbId)
        {
            Console.WriteLine(bulbId);
            return Ok(await _bulbsService.SwitchBulb(bulbId, switchBulbDto.SwitchState));
        }
        [HttpPost("dim/{bulbId}")]
        public async Task<IActionResult> DimBulb([FromBody] IBulbRequest dimBulbDto, string bulbId)
        {
            return Ok(await _bulbsService.DimBulb(bulbId, dimBulbDto));
        }
        
    }
}