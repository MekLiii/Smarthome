using Smarthome.Bulbs.interfaces;

namespace Smarthome.interfaces;

public interface Device
{
    public string DeviceId { get; set; }
    public DeviceType DeviceType { get; set; }
    public string RoomId { get; set; }
    public string DeviceIp { get; set; }
    public int DevicePort { get; set; }
    public string DeviceName { get; set; }
    public Endpoints Endpoints { get; set; }
}

public interface Endpoints
{
    string Info { get; set; }
    string SwitchBulb { get; set; }
    string Dimmable { get; set; }
}