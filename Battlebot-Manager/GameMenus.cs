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
            List<Robot> botList = new List<Robot>();
            for (int i = 0; i < campaign.Team.Robots.Count; i++)
            {
                if (campaign.Team.Robots[i].Weight <= events[response-1].Template.WeightLimit)
                {
                    botList.Add(campaign.Team.Robots[i]);
                }
            }
            List<string> botNames = new List<string>();
            foreach (Robot r in botList)
            {
                botNames.Add(r.Name);
            }
            botNames.Add("attend as a spectator");
            int selectBot = Prompt(botNames.ToArray())-1;
            Console.WriteLine();
            Console.WriteLine();

            if (selectBot == botNames.Count-1)
            {
                events[response-1].Start(null, campaign);
            }
            else
            {
                if (campaign.Cash < events[response-1].Template.EntryFee)
                {
                    Console.WriteLine("You don't have enough cash to enter this event.");
                    Console.WriteLine();
                    Console.WriteLine();
                    return;
                }

                campaign.Cash -= events[response - 1].Template.EntryFee;
                botList[selectBot].Team = campaign.Team;
                events[response-1].Start(botList[selectBot], campaign);
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
        bool quit = false;
        while (!quit)
        {
            if (campaign.Team.Robots.Count == 0)
            {
                if (PromptYN("You have no robots, build a new one?"))
                {
                    NewBot(campaign);
                }
                return;
            }
            
            // List robots
            Console.WriteLine("##| WT |PL| NAME");
            Console.WriteLine("――┼――――┼――┼――――――――――――――――");
            for (int i = 0; i < campaign.Team.Robots.Count; i++)
            {
                Robot bot = campaign.Team.Robots[i];
                Console.WriteLine($"{(i+1).ToString().PadRight(2)}|" +
                                  $"{bot.Weight.ToString().PadRight(4)}|" +
                                  $"{bot.Strongness.ToString().PadRight(2)}| " +
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
                    NewBot(campaign);
                    break;
                case 2: // upgrade existing robot
                    UpgradeBot(campaign);
                    break;
                case 3: // rename existing robot
                    RenameBot(campaign);
                    break;
                case 4: // return to top level menu
                    quit = true;
                    break;
            }
        }
    }

    private static void NewBot(Campaign campaign)
    {
        Robot newbot = new Robot();
        int weight;
        while (true)
        {
            Console.WriteLine("Building new robot. Enter Weight:");
            Console.Write("?> ");
            if (int.TryParse(Console.ReadLine(), out weight))
            {
                // check if the player is cheesing weight
                if (weight < 10 || weight > 500)
                {
                    Console.WriteLine();
                    Console.WriteLine("Not a valid response!");
                    Console.WriteLine();
                    continue;
                }
                Console.WriteLine();
                
                
                int cost = weight * weight / 2;
                if (cost > campaign.Cash)
                {
                    Console.WriteLine($"This bot would cost ${cost} to build,");
                    Console.WriteLine($"You only have ${campaign.Cash}");
                    Console.WriteLine();

                    if (!PromptYN("Start again?"))
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (PromptYN($"This bot will cost ${cost} to build,\n" +
                             $"you have ${campaign.Cash}, continue?"))
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    campaign.Cash -= cost;
                    Console.WriteLine("Robot built! Give it a name:");
                    Console.Write("?> ");
                    newbot.Weight = weight;
                    newbot.Name = Console.ReadLine();
                    campaign.Team.Robots.Add(newbot);
                    return;
                }
                else
                {
                    Console.WriteLine();
                    if (!PromptYN("Start again?"))
                    {
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Not a valid response!");
                Console.WriteLine();
            }
        }
    }

    private static void UpgradeBot(Campaign campaign)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Which bot to upgrade?");
        List<string> upgradeList = new List<string>();
        foreach (Robot bot in campaign.Team.Robots)
        {
            upgradeList.Add(bot.Name);
        }

        int response = Prompt(upgradeList.ToArray());
        Console.WriteLine();
        Console.WriteLine();

        // store selected bot
        Robot sBot = campaign.Team.Robots[response - 1];
        int cost = (int)Math.Pow(2, sBot.Strongness) * sBot.Weight * sBot.Weight / 2;

        if (sBot.Strongness >= 10)
        {
            Console.WriteLine("This bot has been fully upgraded.");
            return;
        }
        

        if (cost > campaign.Cash)
        {
            Console.WriteLine($"This upgrade would cost ${cost},");
            Console.WriteLine($"You only have ${campaign.Cash}");
            Console.WriteLine();

            if (PromptYN("Start again?"))
            {
                UpgradeBot(campaign);
            }
            return;
        }
        
        if (PromptYN($"Upgrading {sBot.Name} will cost ${cost},\n" +
                     $"you have ${campaign.Cash}, continue?"))
        {
            campaign.Cash -= cost;
            campaign.Team.Robots[response - 1].Strongness++;
            Console.WriteLine();
            Console.WriteLine("Upgrade Complete!");
        }
    }

    private static void RenameBot(Campaign campaign)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Rename which bot?");
        List<string> upgradeList = new List<string>();
        foreach (Robot bot in campaign.Team.Robots)
        {
            upgradeList.Add(bot.Name);
        }

        int response = Prompt(upgradeList.ToArray());
        Console.WriteLine();
        Console.WriteLine();
        
        Console.WriteLine("Enter new name:");
        Console.Write("?> ");
        campaign.Team.Robots[response - 1].Name = Console.ReadLine();
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