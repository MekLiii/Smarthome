namespace Smarthome.RollerShade.interfaces;

public interface IRollerShadeService
{
    public void Open(string topic);
    public void Close(string topic);
}