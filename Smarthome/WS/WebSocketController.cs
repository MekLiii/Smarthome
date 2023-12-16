using Microsoft.AspNetCore.Mvc;
using Smarthome.WS.interfaces;

[ApiController]
[Route("api/[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;

    public WebSocketController(IWebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }


    [HttpGet("connect/{roomId}")]
    public async Task<IActionResult> Connect(int roomId)
    {
        try
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketService.HandleWebSocket(HttpContext, webSocket, roomId);
            
            return new EmptyResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
        }
    }
}