using Microsoft.AspNetCore.Mvc;
using Smarthome.WS.interfaces;

namespace Smarthome.WS
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly IWebSocketService _webSocketService;
        public WebSocketController(IWebSocketService webSocketService) => _webSocketService = webSocketService;

        [HttpGet("connect")]
        public async Task<IActionResult> Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketService.HandleWebSocket(HttpContext, webSocket);
                return new EmptyResult();
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}