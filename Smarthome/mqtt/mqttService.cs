using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Smarthome.mqtt.interfaces;

namespace Smarthome.mqtt;

public class MqttService : IMqttService
{
    private static readonly MqttFactory Factory = new();
    private readonly IMqttClient _mqttClient = Factory.CreateMqttClient();
    private const string BrokerServer = "192.168.0.104";

    public async Task ConnectMqttAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(BrokerServer)
            .Build();
        await _mqttClient.ConnectAsync(options, CancellationToken.None);
        Console.WriteLine("### CONNECTED ###");
    }

    public async Task SubscribeTopicAsync(string topic)
    {
        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());

        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };
    }
}