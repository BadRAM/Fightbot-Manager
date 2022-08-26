using System.Text.Json.Serialization;

namespace Battlebot_Manager;

public class Robot
{
    [JsonInclude] public string Name = "BotName";
    [JsonInclude] public int Weight;
    [JsonInclude] public int Strongness;
    [JsonInclude] public List<BotModule> Modules = new List<BotModule>();
    public Team Team;

    public Robot() { }

    public Robot(string name, int weight, int strongness)
    {
        Name = name;
        Weight = weight;
        Strongness = strongness;
        Modules = new List<BotModule>();
    }
}