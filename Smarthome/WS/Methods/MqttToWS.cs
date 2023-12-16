using System.Text;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Smarthome.mqtt.interfaces;
using Smarthome.WS.interfaces;

namespace Smarthome.WS.Methods;

public class MqttToWs
{
    private readonly IMqttService _mqttService;
    private readonly IWebSocketService _webSocketService;
    private readonly int _roomId;

    public MqttToWs(IMqttService mqttService, IWebSocketService webSocketService, int roomId)
    {
        _mqttService = mqttService;
        _webSocketService = webSocketService;
        _roomId = roomId;
    }

    public async Task SubscribeTopicAsync()
    {
        try
        {
            var topics = Topics.GetTopics(_roomId);
            if (topics.Count == 0)
            {
                throw new Exception("No topics found");
            }
            

            var mqttSubscribeOptions = _mqttService.GetMqttFactory().CreateSubscribeOptionsBuilder();

            foreach (var topic in topics)
            {
                mqttSubscribeOptions.WithTopicFilter(topic.Topic, MqttQualityOfServiceLevel.ExactlyOnce);
            }

            var subscribeOptions = mqttSubscribeOptions.Build();

            await _mqttService.GetMqttClient().SubscribeAsync(subscribeOptions);


            _mqttService.GetMqttClient().ApplicationMessageReceivedAsync += e =>
            {
                var topicEntry = topics.Find(topic => topic.Topic == e.ApplicationMessage.Topic);

                if (topicEntry == null)
                {
                    return Task.CompletedTask;
                }

                _webSocketService.SendMessage(new SendMessageDto<object>
                {
                    Type = topicEntry.Type,
                    Payload = JsonConvert.DeserializeObject(
                        Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment ))
                });


                return Task.CompletedTask;
            };
        }
        catch (Exception e)
        {
            _webSocketService.SendMessage(new SendMessageDto<string>
            {
                Type = "Info",
                Case = CaseEnum.Error,
                Message = e.Message,
                Payload = "No room found"
            });
        }
    }
}