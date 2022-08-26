using System.Text.Json.Serialization;

namespace Battlebot_Manager;

public class Campaign
{
    [JsonInclude] public int Month;
    [JsonInclude] public int Cash;
    [JsonInclude] public Team Team;
    [JsonInclude] public List<BotModule> Inventory;

    // This is necessary for deserialization
    public Campaign() {}
    
    public Campaign(string playerTeamName)
    {
        Month = 0;
        Cash = 1000;
        // PlayerBots = new List<Robot>();
        Inventory = new List<BotModule>();
        Team = new Team();
        Team.Name = playerTeamName;
        Team.Robots = new List<Robot>() { new Robot("Sneed", 25, 10) };
    }
}