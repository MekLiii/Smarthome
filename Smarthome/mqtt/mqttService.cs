using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Smarthome.Bulbs.interfaces;
using Smarthome.mqtt.interfaces;
using Smarthome.Rooms.interfaces;
using Smarthome.WS.interfaces;

namespace Smarthome.mqtt;

public class MqttService : IMqttService
{
    private static readonly MqttFactory Factory = new();
    private readonly IMqttClient MqttClient = Factory.CreateMqttClient();
    private const string BrokerServer = "192.168.0.104";

    public async Task ConnectMqttAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(BrokerServer)
            .Build();

        await MqttClient.ConnectAsync(options, CancellationToken.None);
        Console.WriteLine("### CONNECTED ###");
    }
    public MqttFactory GetMqttFactory()
    {
        return Factory;
    }
    public IMqttClient GetMqttClient()
    {
        return MqttClient;
    }

    
}