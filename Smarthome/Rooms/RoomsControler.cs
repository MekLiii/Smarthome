using Microsoft.AspNetCore.Mvc;
using Smarthome.Bulbs.interfaces;
using Smarthome.WS.interfaces;


namespace Smarthome.Rooms
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _roomsService;
        private readonly IWebSocketService _webSocketService;

        public RoomsController(IRoomsService roomsService, IWebSocketService webSocketService)
        {
            _roomsService = roomsService;
            _webSocketService = webSocketService;
        }

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