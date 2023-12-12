using Microsoft.AspNetCore.Mvc;
using Smarthome.Rooms.interfaces;
using Smarthome.WS.interfaces;


namespace Smarthome.Rooms
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _roomsService;
        public RoomsController(IRoomsService roomsService) => _roomsService = roomsService; 
        
        [HttpGet]
        public IActionResult GetRooms()
        {
            var rooms = _roomsService.GetRooms();
            return Ok(rooms);
        }
        [HttpGet("{roomId}")]
        public IActionResult GetRoomById(int roomId)
        {
            var room = _roomsService.GetRoomById(roomId);
            return Ok(room);
        }
    }
}