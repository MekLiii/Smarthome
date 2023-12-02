namespace Smarthome.Bulbs.interfaces;

public interface IBulbsService
{
    Task<List<IBulbInfoData>> GetBulbsInfo(int? roomId);
    Task<SwitchResponse> SwitchBulb(string bulbId, string switchState);
    Task<ResponseFromBulb> DimBulb(string bulbId, IBulbRequest dimBulbDto);
    
}