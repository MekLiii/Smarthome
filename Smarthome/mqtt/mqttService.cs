using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Smarthome.mqtt.interfaces;

namespace Smarthome.mqtt;

public class mqttService : ImqttService
{
    public IMqttClient MqttClient = factory.CreateMqttClient();
    private string brokerServer = "192.168.0.104";
    static MqttFactory factory = new();
    
    public async Task ConnectMqttAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerServer)
            .Build();
        await MqttClient.ConnectAsync(options, CancellationToken.None);
        Console.WriteLine("### CONNECTED ###");
    }
    
    public async Task SubscribeTopicAsync(string topic)
    {
        await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());

        MqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };
    }

    
    
}