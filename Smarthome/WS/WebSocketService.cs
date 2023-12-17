using Smarthome.WS.interfaces;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Smarthome.Bulbs.interfaces;
using Smarthome.mqtt.interfaces;
using Smarthome.WS.Methods;

namespace Smarthome.WS;

public class WebSocketService : IWebSocketService
{
    private WebSocket? _webSocket;
    private int _roomId;


    private readonly IMqttService _mqttService;
    private readonly IBulbsService _bulbsService;

    public WebSocketService(IMqttService mqttService, IBulbsService bulbsService)
    {
        _mqttService = mqttService;
        _bulbsService = bulbsService;
    }

    public int GetRoomId()
    {
        return _roomId;
    }

    public async Task HandleWebSocket(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
    {
        _webSocket = webSocket;


        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType != WebSocketMessageType.Text)
            {
                return;
            }
            
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var roomId = JsonConvert.DeserializeObject<Dictionary<string, int>>(message);
            if (roomId.ContainsKey("roomId"))
            {
                Console.WriteLine("RoomId" + roomId["roomId"]);
                _roomId = roomId["roomId"];
            }

            var currentRoomId = _roomId;
            SendMessage(new SendMessageDto<int>
            {
                Type = "INFO",
                Message = "Connected to room " + currentRoomId,
                Payload = _roomId
            });
            var bulbsInfo = await _bulbsService.GetBulbsInfo(currentRoomId);
            SendMessage(new SendMessageDto<List<IBulbInfoData>>
            {
                Type = "Bulbs",
                Payload = bulbsInfo
            });
            var mqttToWs = new MqttToWs(_mqttService, this, currentRoomId);
            await mqttToWs.SubscribeTopicAsync();
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public async void SendMessage<T>(SendMessageDto<T> message)
    {
        try
        {
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            if (_webSocket == null)
            {
                Console.WriteLine("WebSocket is null");
                return;
            }

            await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}