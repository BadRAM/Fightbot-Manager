using System.Text.Json.Serialization;

namespace Battlebot_Manager;

public class Campaign
{
    [JsonInclude] public int Month;
    [JsonInclude] public int Cash;
    [JsonInclude] public Team Team = new Team();
    [JsonInclude] public List<BotModule> Inventory = new List<BotModule>();

    // This is necessary for deserialization
    public Campaign() {}
    
    public Campaign(string playerTeamName)
    {
        Month = 0;
        Cash = 1000;
        // PlayerBots = new List<Robot>();
        Inventory = new List<BotModule>();
        Team.Name = playerTeamName;
    }
}