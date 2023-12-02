namespace Smarthome.Bulbs.interfaces;

public class IBulbsInfoResponse
{
    int Seq { get; set; }
    int Error { get; set; }
    BulbInfoData Data { get; set; }
}

public class IBulbInfoData
{
    public string Switch { get; set; }
    public int SlowlyLit { get; set; }
    public int SlowlyDimmed { get; set; }
    public string Startup { get; set; }
    public string Ltype { get; set; }
    public colorType colorType { get; set; }
    public int SignalStrength { get; set; }
    public string DeviceId { get; set; }
    public string DeviceName { get; set; }
    public bool? error { get; set; }
}
public class IBulbResponse
{
    public int Seq { get; set; }
    public int Error { get; set; }
    public IBulbInfoData Data { get; set; }
}

public interface IWhiteData
{
    int Br { get; set; }
    int Ct { get; set; }
}

public class BulbsInfoResponse : IBulbsInfoResponse
{
    public int Seq { get; set; }
    public int Error { get; set; }
    public BulbInfoData Data { get; set; }
}

public class BulbInfoData : IBulbInfoData
{
    public string FwVersion { get; set; }
    public string Switch { get; set; }
    public int SlowlyLit { get; set; }
    public int SlowlyDimmed { get; set; }
    public string Startup { get; set; }
    public string Ltype { get; set; }
    public colorType ColorType { get; set; }
    public int SignalStrength { get; set; }
    public string Ssid { get; set; }
    public string Bssid { get; set; }
    public string DeviceId { get; set; }
}

public class colorType : IWhiteData
{
    public int Br { get; set; }
    public int Ct { get; set; }
}