namespace Smarthome.Bulbs.interfaces;

public interface IRoomsService
{
    public List<Room> GetRooms();
    public RoomDetails GetRoomById(int id);
}