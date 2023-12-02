namespace Smarthome.Bulbs.interfaces;

public interface IRoomsService
{
    public List<Room> GetRooms();
    public Room GetRoomById(int id);
}