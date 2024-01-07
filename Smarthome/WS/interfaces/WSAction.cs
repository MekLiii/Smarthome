namespace Smarthome.WS.interfaces;

public class WSAction
{
    public string Action { get; set; }
    public string? Payload { get; set; }
    public int RoomId { get; set; }
}