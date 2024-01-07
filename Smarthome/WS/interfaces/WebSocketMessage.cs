namespace Smarthome.WS.interfaces;

public class WebSocketMessage
{
    public int RoomId { get; set; }
    public ActionDetails Action { get; set; }

    public class ActionDetails
    {
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}