using System.Text;
using MQTTnet;
using MQTTnet.Client;


class MqttZigbeeClient
{
    public async void mqtt(string comPort)
    {
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("192.168.0.104") // Use the appropriate MQTT broker address
            .Build();
        await mqttClient.ConnectAsync(options, CancellationToken.None);

        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("zigbee2mqtt/bridge/state").Build());
        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("zigbee2mqtt/bridge/config").Build());
        var response =
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("zigbee2mqtt/0x84ba20fffedc4440")
                .Build());


        Console.WriteLine("MQTT client subscribed to topics.");
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };

        // The response contains additional data sent by the server after subscribing.
        Console.WriteLine("### SUBSCRIBED ###");
        foreach (var item in response.Items)
        {
            Console.WriteLine("Topic: " + item.TopicFilter.Topic);
            Console.WriteLine("Quality of Service: " + item.TopicFilter.RetainHandling);
        }

        Console.WriteLine("Server response: " + response.Items.ToString());
    }
}