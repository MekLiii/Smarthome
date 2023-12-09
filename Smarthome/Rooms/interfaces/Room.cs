namespace Smarthome.Bulbs.interfaces;

public class Room
{
    public string Name { get; set; }
    public int Id { get; set; }
}

public class RoomDetails
{
    public string Name { get; set; }
    public int Id { get; set; }
    public RoomDevices[]? ZigbeeDevices { get; set; }
}

public class RoomDevices
{
    public string Name { get; set; }
    public string Topic { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
}