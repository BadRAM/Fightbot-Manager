using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Battlebot_Manager;

// Teams have multiple robots
public class Team
{
    [JsonInclude] public string Name;
    [JsonInclude] public List<Robot> Robots = new List<Robot>();
    [JsonInclude] public List<Trophy> Trophies = new List<Trophy>();
    

    // pick a robot to send to tournament.
    // currently picks heaviest bot within weight limit.
    public Robot PickRobot(int weightLimit)
    {
        Robot bot = Robots[0];
        foreach (Robot r in Robots)
        {
            if (r.Weight > bot.Weight && r.Weight <= weightLimit)
            {
                bot = r;
            }
        }
        return bot;
    }
}