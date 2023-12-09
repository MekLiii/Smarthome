using Newtonsoft.Json;
using Smarthome.Bulbs.interfaces;

namespace Smarthome.Rooms;

public class RoomsService : IRoomsService
{
    private static List<T> LoadRoomsFromJson<T>(string jsonFilePath, bool details = false)
    {
        var json = File.ReadAllText(jsonFilePath);

        if (details)
        {
            var roomDetailsDataList = JsonConvert.DeserializeObject<List<RoomDetails>>(json);
            if (roomDetailsDataList == null)
                throw new Exception("No rooms found");
            return roomDetailsDataList.Cast<T>().ToList();
        }

        var roomDataList = JsonConvert.DeserializeObject<List<Room>>(json);
        if (roomDataList == null)
            throw new Exception("No room found");
        return roomDataList.Cast<T>().ToList();
    }

    public List<Room> GetRooms()
    {
        return LoadRoomsFromJson<Room>("rooms.json");
    }

    public RoomDetails GetRoomById(int id)
    {
        var rooms = LoadRoomsFromJson<RoomDetails>("rooms.json", true);
        var room = rooms.FirstOrDefault(room => room.Id == id);
        if (room == null)
        {
            throw new Exception($"No room found with id {id}");
        }

        return room;
    }
}