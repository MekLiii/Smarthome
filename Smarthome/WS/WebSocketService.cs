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
    // private Dictionary<string, T> getRowFromMessage<T>(string messageKey)
    // {
    //     var messageDict = JsonConvert.DeserializeObject<Dictionary<string, T>>(message);
    //     if (messageDict != null && messageDict.ContainsKey("row"))
    //     {
    //         return messageDict[messageKey];
    //     }
    //     return null;
    // }

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

            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var message = JsonConvert.DeserializeObject<WebSocketMessage>(messageString);
            if (message == null)
            {
                return;
            }

            var roomId = message.RoomId;

            _roomId = roomId;
            ProcessMessage(message);
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

    private async void ProcessMessage(WebSocketMessage message)
    {
        Console.WriteLine(message.Action.Type);
        if (message.Action.Type == "info")
        {
            var currentRoomId = _roomId;

            var mqttToWs = new MqttToWs(_mqttService, this, currentRoomId);
            await mqttToWs.SubscribeTopicAsync();

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
        }

        if (message.Action.Type == "RollerShade")
        {
            if (message.Action.Payload == "OPEN")
            {
                Console.WriteLine($"Opening roller shade in room {message.RoomId}");
            }
        }
    }
}