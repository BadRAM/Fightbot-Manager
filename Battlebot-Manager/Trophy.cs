using System.Text.Json.Serialization;

namespace Battlebot_Manager;

public class Trophy
{
    [JsonInclude] public string Name = "";
    [JsonInclude] public string EventName = "";
    [JsonInclude] public string BotName = "";
    [JsonInclude] public string TeamName = "";
    [JsonInclude] public int Month = 0;

    public Trophy() { }
    
}