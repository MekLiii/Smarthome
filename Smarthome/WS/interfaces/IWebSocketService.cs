namespace Smarthome.WS.interfaces;

public interface IWebSocketService
{
    public Task HandleWebSocket(HttpContext context, System.Net.WebSockets.WebSocket webSocket, int roomId);
    public void SendMessage<TPayload>(SendMessageDto<TPayload> message);
    public int GetRoomId();
}