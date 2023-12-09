using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smarthome.Bulbs.interfaces;
using Smarthome.mqtt.interfaces;


namespace Smarthome.mqtt
{
    [ApiController]
    [Route("api/[controller]")]
    public class mqttContoler : ControllerBase
    {
        private readonly ImqttService  _imqttService;

        public mqttContoler(ImqttService mqttService) => _imqttService = mqttService;

        [HttpGet("{topic}")]
        public IActionResult GetThermometer(string topic)
        {
            
            return Ok("1");
        }
        
    }
}
