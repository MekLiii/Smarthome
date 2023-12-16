using Newtonsoft.Json;
using Smarthome.Rooms.interfaces;

namespace Smarthome.WS.Methods
{
    class LoadTopicsFromJson
    {
        private const string RoomDataListFromJson = "rooms.json";

        public static List<RoomTopic> LoadTopics(int roomId)
        {
            var json = File.ReadAllText(RoomDataListFromJson);

            var roomsDetailsDataList = JsonConvert.DeserializeObject<List<RoomDetails>>(json);
            if (roomsDetailsDataList == null)
            {
                throw new Exception("No room found");
            }

            var roomDataList = roomsDetailsDataList.Find(
                room => room.Id == roomId);
            Console.WriteLine(roomDataList);
            if (roomDataList == null)
            {
                throw new Exception("No room found");
            }

            if (roomDataList.ZigbeeDevices == null)
            {
                throw new Exception("No zigbee devices found");
            }

            var roomTopics = roomDataList.ZigbeeDevices.Select(device =>
                new RoomTopic
                {
                    Topic = device.Topic,
                    Type = device.Type
                }
            ).ToList();


            if (roomDataList == null)
            {
                throw new Exception("No room found");
            }

            return roomTopics;
        }
    }
}