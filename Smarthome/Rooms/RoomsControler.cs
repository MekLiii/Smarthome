using Microsoft.AspNetCore.Mvc;
using Smarthome.Bulbs.interfaces;


namespace Smarthome.Rooms
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _roomsService;

        public RoomsController(IRoomsService bulbsService) => _roomsService = bulbsService;

        [HttpGet]
        public IActionResult GetBulbs()
        {
            var bulbs = _roomsService.GetRooms();
            Console.WriteLine("test");
            return Ok(bulbs);
        }
        [HttpGet("{id}")]
        public IActionResult GetBulb(int id)
        {
            var bulb = _roomsService.GetRoomById(id);
            return Ok(bulb);
        }
    }
}