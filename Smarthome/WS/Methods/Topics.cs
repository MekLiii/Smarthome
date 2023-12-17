using Smarthome.Rooms.interfaces;

namespace Smarthome.WS.Methods;

public class Topics
{
    public int RoomId { get; set; }
    public Topics(int roomId)
    {
        RoomId = roomId;
    }
    public List<RoomTopic> GetTopics()
    {
        try
        {
            Console.WriteLine("RoomId topics" + RoomId);
            var LoadTopicsFromJson = new LoadTopicsFromJson(RoomId);
            return LoadTopicsFromJson.LoadTopics();
        }
        catch (Exception e)
        {
            return new List<RoomTopic>();
            throw new Exception(e.Message);
        }
    }
}