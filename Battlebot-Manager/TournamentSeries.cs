using System.Text.Json.Serialization;

namespace Battlebot_Manager;

public class TournamentSeries
{
    [JsonInclude] public string Name;
    [JsonInclude] public int Size;
    [JsonInclude] public int WeightLimit;
    [JsonInclude] public int TeamSize;
    [JsonInclude] public int EntryFee;
    [JsonInclude] public int Prize;
    [JsonInclude] public Trophy Trophy = new Trophy();
    [JsonInclude] public List<SeriesEntry> Entries = new List<SeriesEntry>();
    
    public class SeriesEntry
    {
        [JsonInclude] public string Name;
        [JsonInclude] public int Month;

        public SeriesEntry(string name, int month)
        {
            Name = name;
            Month = month;
        }
    }
    
    public string GetEventName(int eventindex, int monthsSinceStart)
    {
        return string.Format(Entries[eventindex].Name, 
            Content.StartDate.AddMonths(monthsSinceStart).ToString("MMMM, yyyy"));
    }

    public string GetEventName(int month)
    {
        foreach (SeriesEntry i in Entries)
        {
            if (i.Month-1 == month % 12)
            {
                return string.Format(i.Name, Content.StartDate.AddMonths(month).ToString("MMMM, yyyy"));
            }
        }

        return "Erroneously requested event from " + Name;
    }
}
