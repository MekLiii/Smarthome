using Smarthome.WS.interfaces;
using System.Net.WebSockets;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Smarthome.mqtt.interfaces;
using Smarthome.Rooms.interfaces;

namespace Smarthome.WS;

public class WebSocketService : IWebSocketService
{
    private WebSocket? _webSocket;
    private int _roomId;

    private readonly IMqttService _mqttService;

    public WebSocketService(IMqttService mqttService)
    {
        _mqttService = mqttService;
       
    }

    public async Task HandleWebSocket(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
    {
        _webSocket = webSocket;


        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var roomId = JsonConvert.DeserializeObject<Dictionary<string, int>>(message);
                if (roomId != null)
                {
                    _roomId = roomId["roomId"];
                    await SubscribeTopicAsync();
                }
                SendMessage("Connected to room " + _roomId);
            }
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public async void SendMessage(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
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

    public int GetRoomId()
    {
        return _roomId;
    }

    private List<RoomTopic> LoadTopicsFromJson()
    {
        var json = File.ReadAllText("rooms.json");


        var roomsDetailsDataList = JsonConvert.DeserializeObject<List<RoomDetails>>(json);
        if (roomsDetailsDataList == null)
        {
            throw new Exception("No room found");
        }

        var roomDataList = roomsDetailsDataList.Find(
            room => room.Id == GetRoomId());
        if (roomDataList == null)
        {
            throw new Exception("No topic found");
        }

        var roomTopics = roomDataList.ZigbeeDevices.Select(device =>
            new RoomTopic
            {
                Topic = device.Topic,
                Type = device.Type
            }
        ).ToList();


        if (roomDataList == null)
            throw new Exception("No rooms found");
        return roomTopics;
    }

    private async Task SubscribeTopicAsync()
    {
        var topics = LoadTopicsFromJson();

        var mqttSubscribeOptions = _mqttService.GetMqttFactory().CreateSubscribeOptionsBuilder();

        foreach (var topic in topics)
        {
            mqttSubscribeOptions.WithTopicFilter(topic.Topic, MqttQualityOfServiceLevel.ExactlyOnce);
        }

        var subscribeOptions = mqttSubscribeOptions.Build();

        await _mqttService.GetMqttClient().SubscribeAsync(subscribeOptions);

        _mqttService.GetMqttClient().ApplicationMessageReceivedAsync += e =>
        {
            SendMessage(JsonConvert.SerializeObject(new
            {
                topic = e.ApplicationMessage.Topic,
                payload = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)),
                type = topics.Find(topic => topic.Topic == e.ApplicationMessage.Topic)?.Type
            }));
            return Task.CompletedTask;
        };
    }
}