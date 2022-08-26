using System.Text.Json;

namespace Battlebot_Manager;

public static class Content
{
    public static DateOnly StartDate = new DateOnly(2000, 1, 1);
    public static List<Team> Teams = new List<Team>();
    public static List<TournamentSeries> TournamentPresets = new List<TournamentSeries>();

    static Content()
    {
        // Get path to program directory
        string path = Path.GetFullPath(@"..\..\..\");

        // load tournament presets
        using FileStream tournamentPresetsFile = File.OpenRead(path + "Content/TournamentSeries.json");
        TournamentPresets = JsonSerializer.Deserialize<List<TournamentSeries>>(tournamentPresetsFile);
        tournamentPresetsFile.Dispose();

        // load teams and their robots
        using FileStream teamsFile = File.OpenRead(path + "Content/Teams.json");
        Teams = JsonSerializer.Deserialize<List<Team>>(teamsFile);
        teamsFile.Dispose();
    }

    public static List<string> CheckEvents(int monthsSinceStart)
    {
        List<string> output = new List<string>();

        for (int i = 0; i < TournamentPresets.Count; i++)
        {
            for (int j = 0; j < TournamentPresets[i].Entries.Count; j++)
            {
                if (TournamentPresets[i].Entries[j].Month - 1 == monthsSinceStart % 12)
                {
                    output.Add(TournamentPresets[i].GetEventName(j, monthsSinceStart));
                }
            }
        }

        return output;
    }

    public static List<Tournament> GetEvents(int monthsSinceStart)
    {
        List<Tournament> output = new List<Tournament>();

        for (int i = 0; i < TournamentPresets.Count; i++)
        {
            for (int j = 0; j < TournamentPresets[i].Entries.Count; j++)
            {
                if (TournamentPresets[i].Entries[j].Month - 1 == monthsSinceStart % 12)
                {
                    output.Add(new Tournament(TournamentPresets[i], monthsSinceStart));
                }
            }
        }

        return output;
    }

    public static Robot GetRobot(int weightLimit)
    {
        Team t = Teams[Random.Shared.Next(Teams.Count - 1)];
        Robot r = t.PickRobot(weightLimit);
        r.Team = t;
        return r;
    }

}