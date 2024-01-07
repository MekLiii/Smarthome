using Smarthome.mqtt.interfaces;
using Smarthome.RollerShade.interfaces;

namespace Smarthome.RollerShade;

public class RollerShadeService : IRollerShadeService
{
    private readonly IMqttService _mqttService;

    public RollerShadeService(IMqttService mqttService)
    {
        _mqttService = mqttService;
    }
    public void Open(string topic)
    {
        _mqttService.Publish($"{topic}/set", "{'state':'OPEN'}");
    }
    public void Close(string topic)
    {
        _mqttService.Publish($"{topic}/set", "{'state':'CLOSE'}");
    }

}