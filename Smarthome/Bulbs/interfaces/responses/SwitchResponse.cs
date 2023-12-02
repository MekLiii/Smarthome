namespace Smarthome.Bulbs.interfaces;


public class SwitchResponse 
{
    public int Seq { get; set; }
    public int Error { get; set; }
    public string @switch { get; set; }
}
public class ResponseFromBulb
{
    public int Seq { get; set; }
    public int Error { get; set; }
}