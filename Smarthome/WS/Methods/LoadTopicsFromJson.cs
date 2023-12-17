using Newtonsoft.Json;
using Smarthome.Rooms.interfaces;


class LoadTopicsFromJson
{
    private const string RoomDataListFromJson = "rooms.json";
    private int roomId;
    public LoadTopicsFromJson(int roomId)
    {
        this.roomId = roomId;
    }

    public List<RoomTopic> LoadTopics()
    {
        try
        {
            var json = File.ReadAllText(RoomDataListFromJson);
            var roomsDetailsDataList = JsonConvert.DeserializeObject<List<RoomDetails>>(json);

            if (roomsDetailsDataList == null)
            {
                throw new Exception("No room details found in the JSON file.");
            }

            var roomDataList = roomsDetailsDataList.Find(room => room.Id == roomId);

            if (roomDataList == null)
            {
                throw new Exception($"No room found with Id {roomId}.");
            }

            if (roomDataList.ZigbeeDevices == null)
            {
                throw new Exception($"No zigbee devices found for RoomId {roomId}.");
            }

            var roomTopics = roomDataList.ZigbeeDevices.Select(device =>
                new RoomTopic
                {
                    Topic = device.Topic,
                    Type = device.Type
                }).ToList();

            Console.WriteLine($"Loaded topics for RoomId {roomId}: {JsonConvert.SerializeObject(roomTopics)}");
            return roomTopics;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading topics for RoomId {roomId}: {e.Message}");
            // Rethrow the exception to make it more visible.
            throw;
        }
    }
}
