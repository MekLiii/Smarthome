using Smarthome.WS.interfaces;
using System.Net.WebSockets;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Smarthome.mqtt.interfaces;
using Smarthome.Rooms.interfaces;
using Smarthome.WS.Methods;

namespace Smarthome.WS;

public class WebSocketService : IWebSocketService
{
    private static WebSocket? _webSocket;
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
                SendMessage(new SendMessageDto<int>
                {
                    Type = "INFO",
                    Message = "Connected to room " + _roomId,
                    Payload = _roomId
                });
            }
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public async void SendMessage<T>(SendMessageDto<T> message)
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

    public int GetRoomId()
    {
        return _roomId;
    }


    private async Task SubscribeTopicAsync()
    {
        try
        {
            var topics = LoadTopicsFromJson.LoadTopics(_roomId);

            var mqttSubscribeOptions = _mqttService.GetMqttFactory().CreateSubscribeOptionsBuilder();

            foreach (var topic in topics)
            {
                mqttSubscribeOptions.WithTopicFilter(topic.Topic, MqttQualityOfServiceLevel.ExactlyOnce);
            }

            var subscribeOptions = mqttSubscribeOptions.Build();

            await _mqttService.GetMqttClient().SubscribeAsync(subscribeOptions);


            _mqttService.GetMqttClient().ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine(
                    JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)));
                SendMessage(new SendMessageDto<object>
                {
                    Type = topics.Find(topic => topic.Topic == e.ApplicationMessage.Topic)?.Type,
                    Payload = JsonConvert.DeserializeObject(
                        Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment))
                });
                return Task.CompletedTask;
            };
        }
        catch (Exception e)
        {
            SendMessage(new SendMessageDto<string>
            {
                Type = "Info",
                Case = CaseEnum.Error,
                Message = e.Message,
                Payload = "No room found"
            });
        }
    }
}