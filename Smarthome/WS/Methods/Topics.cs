﻿using Smarthome.Rooms.interfaces;

namespace Smarthome.WS.Methods;

public class Topics
{

   
    public static List<RoomTopic> GetTopics(int roomId)
    {
        try
        {
            return LoadTopicsFromJson.LoadTopics(roomId);
        }
        catch (Exception e)
        {
            return new List<RoomTopic>();
            throw new Exception(e.Message);
        }
    }
}