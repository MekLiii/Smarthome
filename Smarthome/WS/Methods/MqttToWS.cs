using System.Text;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Smarthome.mqtt.interfaces;
using Smarthome.Rooms.interfaces;
using Smarthome.WS.interfaces;

namespace Smarthome.WS.Methods
{
    public class MqttToWs
    {
        private readonly IMqttService _mqttService;
        private readonly IWebSocketService _webSocketService;
        private static int _roomId;
        private List<RoomTopic> _currentTopics = new List<RoomTopic>();

        public MqttToWs(IMqttService mqttService, IWebSocketService webSocketService, int roomId)
        {
            _mqttService = mqttService;
            _webSocketService = webSocketService;
            _roomId = roomId;
        }

        private List<RoomTopic> CurrentTopics
        {
            get
            {
                var topicsInstance = new Topics(_roomId);
                Console.WriteLine("RoomId topics " + _roomId);
                _currentTopics = topicsInstance.GetTopics();
                return _currentTopics;
            }
        }
        
        public async Task SubscribeTopicAsync()
        {
            try
            {
                var mqttSubscribeOptions = _mqttService.GetMqttFactory().CreateSubscribeOptionsBuilder();

                foreach (var topic in CurrentTopics)
                {
                    mqttSubscribeOptions.WithTopicFilter(topic.Topic, MqttQualityOfServiceLevel.ExactlyOnce);
                }

                var subscribeOptions = mqttSubscribeOptions.Build();

                await _mqttService.GetMqttClient().SubscribeAsync(subscribeOptions);
                
                _mqttService.GetMqttClient().ApplicationMessageReceivedAsync += async e =>
                {
      

                    var topicEntry = CurrentTopics.Find(topic => topic.Topic == e.ApplicationMessage.Topic);
                    

                    var payload = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    

                    if (CurrentTopics.Exists(topic => topic.Topic == e.ApplicationMessage.Topic))
                    {
                        _webSocketService.SendMessage(new SendMessageDto<object>
                        {
                            Type = topicEntry.Type,
                            Payload = payload
                        });
                    }
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
}