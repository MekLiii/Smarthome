using MQTTnet;
using MQTTnet.Client;
using Smarthome.WS.interfaces;

namespace Smarthome.mqtt.interfaces;

public interface IMqttService
{
    public Task ConnectMqttAsync();
    public MqttFactory GetMqttFactory();
    public IMqttClient GetMqttClient();
    public void Publish(string topic, string payload);
 
}