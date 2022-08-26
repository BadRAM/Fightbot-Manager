using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using static Utils;

namespace Battlebot_Manager;

// This is a collection of static functions to be called by the main game loop.
public static class GameMenus
{
    public static Campaign LoadGame()
    {
        string path = Path.GetFullPath(@"..\..\..\");
        

        string[] saves = Directory.GetFiles(path + "Saves/");
        List<string> saveNames = new List<string>();
        foreach (string s in saves)
        {
            saveNames.Add(s.Replace(path + "Saves/", ""));
        }
        saveNames.Add("New Game");

        Console.WriteLine("Load one of the following Save Files:");
        Console.WriteLine();
        int response = Prompt(saveNames.ToArray());

        if (response == saveNames.Count)
        {
            Console.WriteLine("Enter new team name: ");
            Console.Write("?> ");
            string newSaveName = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine();
            return new Campaign(newSaveName);
        }
        else
        {
            Campaign loadedSave;
            using FileStream saveFile = File.OpenRead(saves[response-1]);
            loadedSave = JsonSerializer.Deserialize<Campaign>(saveFile);
            saveFile.Dispose();
            Console.WriteLine();
            Console.WriteLine();

            
            return loadedSave;
        }
    }
    public static void UpcomingEvents(Campaign campaign)
    {
        List<Tournament> events = Content.GetEvents(campaign.Month);
        List<string> prompts = new List<string>();

        foreach (Tournament e in events)
        {
            prompts.Add(e.Name);
        }
        prompts.Add("Return");

        int response = Prompt(prompts.ToArray());
        Console.WriteLine();
        Console.WriteLine();

        
        // return to top if selected
        if (response == prompts.Count) {return;}
        
        // else inspect selected event
        Console.WriteLine(events[response-1].Name + ":");
        Console.WriteLine($"Part of the {events[response-1].Template.Name} series.");
        Console.WriteLine($"Size: {Math.Pow(2, events[response-1].Template.Size)} entrants, {events[response-1].Template.Size} stages.");
        Console.WriteLine($"Weight Limit: {events[response-1].Template.WeightLimit}");
        Console.WriteLine($"Registration Fee: {events[response-1].Template.EntryFee}");
        Console.WriteLine($"Grand Prize: {events[response-1].Template.Prize}");
        Console.WriteLine();
        
        // prompt player to attend event
        if (PromptYN($"Attend {events[response-1].Name}?"))
        {
            Console.WriteLine();
            Console.WriteLine();
            
            
            // prompt player to select bot from their roster
            Console.WriteLine("Which bot do you want to enter with?");
            List<string> botList = new List<string>();
            for (int i = 0; i < campaign.Team.Robots.Count; i++)
            {
                botList.Add(campaign.Team.Robots[i].Name);
            }
            botList.Add("attend as a spectator");
            int selectBot = Prompt(botList.ToArray())-1;
            Console.WriteLine();
            Console.WriteLine();

            if (selectBot == botList.Count-1)
            {
                events[response-1].Start(null, campaign);
            }
            else
            {
                campaign.Team.Robots[selectBot].Team = campaign.Team;
                events[response-1].Start(campaign.Team.Robots[selectBot], campaign);
            }
            
            campaign.Month++;
            return;
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine();
            return;
        }
    }

    public static void Garage(Campaign campaign)
    {
        while (true)
        {
            // List robots
            Console.WriteLine("##| WT |PL| NAME");
            for (int i = 0; i < campaign.Team.Robots.Count; i++)
            {
                Robot bot = campaign.Team.Robots[i];
                Console.WriteLine($"{(i+1).ToString().PadRight(2)}|" +
                                  $"{bot.Weight.ToString().PadRight(4)}|" +
                                  $"{bot.Strongness.ToString().PadRight(2)}" +
                                  $"{bot.Name}");
            }
            Console.WriteLine();
            
            // Ask the player what to do
            int response = Prompt(new string[] 
            {
                "Build new", 
                "Upgrade", 
                "Rename",
                "Exit"
            });
            Console.WriteLine();
            Console.WriteLine();
            
            
            switch (response)
            {
                case 1: // build new robot
                    Robot newbot = new Robot();
                    while (true)
                    {
                        Console.WriteLine("Building new robot. Enter Weight:");
                        Console.Write("?>");
                        if (int.TryParse(Console.ReadLine(), out newbot.Weight))
                        {
                            break;
                        }
                        Console.WriteLine();
                        Console.WriteLine("Not a valid response!");
                        Console.WriteLine();
                    }
                    
                    Console.WriteLine();
                    Console.WriteLine("Enter Weight");
                    
                    break;
                case 2: // upgrade existing robot
                    break;
                case 3: // rename existing robot
                    break;
                case 4: // return to top level menu
                    break;
            }
        }
    }

    public static void TrophyRoom(Campaign campaign)
    {
        if (campaign.Team.Trophies.Count == 0) 
        {
            Console.WriteLine("You haven't won any trophies yet!");
            Console.WriteLine();
            Console.ReadLine();
            return;
        }

        foreach (Trophy trophy in campaign.Team.Trophies)
        {
            Console.WriteLine(trophy.Name);
            Console.WriteLine(trophy.EventName);
            Console.WriteLine(trophy.BotName);
            Console.WriteLine(trophy.TeamName);
            Console.WriteLine(Content.StartDate.AddMonths(trophy.Month).ToString("MMMM, yyyy"));
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}