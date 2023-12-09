using Smarthome.WS.interfaces;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Smarthome.WS;

public class WebSocketService : IWebSocketService
{
    public async Task HandleWebSocket(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
    {
        // Your WebSocket handling logic goes here
        // Example: Echo back messages
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                    result.EndOfMessage, CancellationToken.None);
            }
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}