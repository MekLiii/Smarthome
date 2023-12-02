namespace Smarthome.Bulbs.interfaces;

public class IBulbRequest
{
    public string ltype { get; set; }
    public ColorTypes colorTypes { get; set; }
}

public class ColorTypes
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
    public int Br { get; set; }
    public int Ct { get; set; }
}