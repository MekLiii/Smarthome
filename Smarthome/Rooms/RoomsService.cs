using Newtonsoft.Json;
using Smarthome.Bulbs.interfaces;

namespace Smarthome.Rooms;

public class RoomsService : IRoomsService
{
    private static List<Room> LoadRoomsFromJson(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath); // Assuming you have a JSON file
        var roomDataList = JsonConvert.DeserializeObject<List<Room>>(json);
        return roomDataList;
    }
    public List<Room> GetRooms()
    {
        return LoadRoomsFromJson("rooms.json");
    }
    public Room GetRoomById(int id)
    {
        var rooms = LoadRoomsFromJson("rooms.json");
        var room = rooms.FirstOrDefault(room => room.Id == id);
        if (room == null)
        {
            throw new Exception($"No room found with id {id}");
        }
        return room;
    }
}