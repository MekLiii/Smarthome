using Smarthome.WS.interfaces;
using System.Net.WebSockets;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Smarthome.Bulbs.interfaces;
using Smarthome.mqtt.interfaces;
using Smarthome.Rooms.interfaces;
using Smarthome.WS.Methods;

namespace Smarthome.WS;

public class WebSocketService : IWebSocketService
{
    private static WebSocket? _webSocket;
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
    
    public async Task HandleWebSocket(HttpContext context, System.Net.WebSockets.WebSocket webSocket, int roomId)
    {
        _webSocket = webSocket;
        _roomId = roomId;

        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;
        SendMessage(new SendMessageDto<int>
        {
            Type = "INFO",
            Message = "Connected to room " + _roomId,
            Payload = _roomId
        });
        var bulbsInfo = await _bulbsService.GetBulbsInfo(_roomId);
        SendMessage(new SendMessageDto<List<IBulbInfoData>>
        {
            Type = "Bulbs",
            Payload = bulbsInfo
        });
        var mqttToWs = new MqttToWs(_mqttService, this, _roomId);
        await mqttToWs.SubscribeTopicAsync();
        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
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
            
                    if (_webSocket.State != WebSocketState.Open)
                    {
                        Console.WriteLine("WebSocket is not open");
                        return;
                    }
            
                    await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true,
                        CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
}