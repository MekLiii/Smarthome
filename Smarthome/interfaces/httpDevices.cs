using Smarthome.interfaces;

namespace Smarthome;

public class Device
{
    public string? DeviceName { get; set; }
    public string DeviceId { get; set; }
    public string DeviceIp { get; set; }
    public int DevicePort { get; set; }
    public Endpoints? Endpoints;
    public DeviceType DeviceType { get; set; }
    public int RoomId { get; set; }
}

public class Endpoints
{
    public string Info { get; set; }
    public string SwitchBulb { get; set; }
    public string Dimmable { get; set; }
}
public interface RoomForBulb
{
    public string Room { get; set; }
    public int RoomId { get; set; }
}

