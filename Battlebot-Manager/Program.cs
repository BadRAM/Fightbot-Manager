// See https://aka.ms/new-console-template for more information


/*
 * TODO:
 * Check Weight Limit
 * Restrict bot weights
 * Restrict max upgrade
 * 
 */


using System.Diagnostics;
using System.Text.Json;
using Battlebot_Manager;

string path = Path.GetFullPath(@"..\..\..\");

var opt = new JsonSerializerOptions
{
    WriteIndented = true
};

//Console.WriteLine(JsonSerializer.Serialize(FightFight, opt));


Campaign campaign = GameMenus.LoadGame();

// MAIN GAME LOOP
bool quit = false;
while (!quit)
{
    // state the current month/year
    DateOnly currentDate = Content.StartDate.AddMonths(campaign.Month);
    Console.WriteLine($"{currentDate.ToString("MMMM yyyy")}");
    Console.WriteLine($"{campaign.Team.Name} has ${campaign.Cash} and {campaign.Team.Trophies.Count} trophies to it's name.");
    Console.WriteLine();
    
    int response = Utils.Prompt(new string[] 
    {
        "Check upcoming events", 
        "Go to the garage", 
        "View Trophies",
        "Wait until next month", 
        "Save & Quit"
    });
    Console.WriteLine();
    Console.WriteLine();

    switch (response)
    {
        case 1: // Check upcoming events
            GameMenus.UpcomingEvents(campaign);
            break;
        
        case 2: // Go to the garage
            GameMenus.Garage(campaign);
            break;
        
        case 3: // View Trophies
            GameMenus.TrophyRoom(campaign);
            break;
        
        case 4: // Wait until next month
            Console.WriteLine("\nZZZ...\n");
            campaign.Month++;
            break;
        
        case 5: // Save & Quit
        {
            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            //Create the file.
            using FileStream createStream = File.Create(path + "/Saves/" + campaign.Team.Name + ".json");
            JsonSerializer.Serialize(createStream, campaign, opt);
            createStream.Dispose();
            quit = true;
            break;
        }
    }
    Thread.Sleep(500);
}
