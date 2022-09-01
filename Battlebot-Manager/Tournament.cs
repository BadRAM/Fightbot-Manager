namespace Battlebot_Manager;

public class Tournament
{
    public string Name;
    private int _stage;
    public List<List<Robot>> _bracket;
    public TournamentSeries Template;
    

    public Tournament(TournamentSeries template, int month)
    {
        Name = template.GetEventName(month);
        Template = template;
        _bracket = new List<List<Robot>>();
        for (int i = 0; i <= template.Size; i++)
        {
            _bracket.Add(new List<Robot>());
        }
        
    }

    //
    // public Tournament(string name, TournamentSeries preset)
    // {
    //     Name = name;
    //     _preset = preset;
    // }


    /* Bracket returns a string representation of the tournament bracket, example:
     *
     * Carpet Shark         ―┐  
     *                       ├―┐
     * Who's Afraid of PLA? ―┘ |
     *                         ├―┐
     * Deadshaft            ―┐ | |
     *                       ├―┘ |
     * Pauldron             ―┘   |
     *                           ├―
     * Carpet Shark         ―┐   |
     *                       ├―┐ |
     * Who's Afraid of PLA? ―┘ | |
     *                         ├―┘
     * Deadshaft            ―┐ |
     *                       ├―┘
     * Pauldron             ―┘
     * 
     */
    
    
    public string Bracket(int stageWidth)
    {
        // stageWidth; // how many dashes wide is each stage of the tournament?
        // find the longest name
        int longestName = 0;
        foreach (Robot bb in _bracket[0])
        {
            if (bb.Name.Length > longestName)
            {
                longestName = bb.Name.Length;
            }
        }

        // populate lines with padded names and spaces
        List<string> lines = new List<string>();
        string line;
        int div;
        for (int i = 0; i < _bracket[0].Count; i++)
        {
            //lines.Add(_bracket[0][i].Name.PadRight(longestName+1));
            line = _bracket[0][i].Name.PadRight(longestName + 1, '―');
            line += "―";
            line += i % 2 == 0 ? "┐".PadRight(stageWidth+1) : "┘".PadRight(stageWidth+1);
            for (int j = 2; j <= Template.Size; j++)
            {
                div = (int)Math.Pow(2, j);
                line += (i + div/4) % div >= div/2 ? "|".PadRight(stageWidth+1) : " ".PadRight(stageWidth+1);
            }
            lines.Add(line);


            if (i != _bracket[0].Count - 1)
            {
                //lines.Add("".PadRight(longestName + 1));
                line = "".PadRight(longestName + 1);
                
                string winner = "";
                //Console.WriteLine("checking if bracket[1].Count (" + _bracket[1].Count + ") is > i/2 (" + i/2 + ")");
                if (_bracket[1].Count > i/2)
                {
                    //Console.WriteLine("check passed");
                    winner = _bracket[1][i / 2].Name.Truncate(stageWidth);
                }
                line += i % 2 == 0 ? (" ├" + winner).PadRight(stageWidth + 2, '―') : "  ".PadRight(stageWidth+2);

                for (int j = 2; j <= Template.Size; j++)
                {
                    div = (int)Math.Pow(2, j);
                    //line += (i + div/4) % div >= div/2 ? "├―" : "  ";

                    if (((i + div / 4) + 1) % div == 0)
                    {
                        line += "┘".PadRight(stageWidth + 1);
                    }
                    else if (((i - div / 4) + 1) % div == 0)
                    {
                        line += "┐".PadRight(stageWidth + 1);
                    }
                    else if (((i + div / 2) + 1) % div == 0)
                    {
                        winner = "";
                        //Console.WriteLine("within for loop checking if bracket[" + (j) + "].Count (" + _bracket[j].Count + ") is > i/div (" + i/div + ")");
                        if (_bracket[j].Count > i/div)
                        {
                            //Console.WriteLine("Check passed!");
                            winner = _bracket[j][i / div].Name.Truncate(stageWidth);
                        }
                        line += ("├" + winner).PadRight(stageWidth + 1, '―');
                    }
                    else if (((i + div / 4) + 1) % div > div / 2)
                    {
                        line += "|".PadRight(stageWidth + 1);
                    }
                    else
                    {
                        line += " ".PadRight(stageWidth + 1);
                    }
                }
                
                lines.Add(line);
            }
        }

        // return the contents of lines as a \n separated string
        string b = "";
        foreach (string l in lines)
        {
            b += l + "\n";
        }
        return b;
    }
    
    public void Start(Robot? playerBot, Campaign campaign)
    {
        // Generate bracket
        List<Robot> bots = Content.GetRobots(Template.WeightLimit);
        for (int i = 0; i < Math.Pow(2, Template.Size)-1; i++)
        {
            _bracket[0].Add(bots.RandomPop());
        }
        // insert player bot at random position in base of bracket
        if (playerBot != null)
        {
            _bracket[0].Insert(Random.Shared.Next(_bracket.Count), playerBot);
        }
        else // if the player does not enter a bot, add the final bot from content like normal
        {
            _bracket[0].Add(bots.RandomPop());
        }
        
        Console.WriteLine("Welcome to " + Name);
        Console.WriteLine("\nThe tournament bracket is:");

        while (true)
        {
            Console.WriteLine(Bracket(2));
            Console.WriteLine();

            // If tournament is finished
            if (NextMatch() && _stage == Template.Size)
            {
                Console.WriteLine("\nThe winner is: " + _bracket[Template.Size][0].Name);
                Console.WriteLine();
                
                // If player Won
                if (_bracket[Template.Size][0] == playerBot)
                {
                    Trophy t = new Trophy();
                    t.Name = Template.Trophy.Name;
                    t.EventName = Name;
                    t.BotName = playerBot.Name;
                    t.TeamName = campaign.Team.Name;
                    t.Month = campaign.Month;
                    campaign.Team.Trophies.Add(t);
                    campaign.Cash += Template.Prize;
                    Console.WriteLine($"You won {Name}! ${Template.Prize} collected!");
                }
                
                break;
            }
            
        }
    }

    public bool NextMatch()
    {
        int i1 = _bracket[_stage+1].Count*2;
        int i2 = i1 + 1;
        
        if (BotBattle.Fight(_bracket[_stage][i1], _bracket[_stage][i2]))
        {
            _bracket[_stage+1].Add(_bracket[_stage][i1]);
        }
        else
        {
            _bracket[_stage+1].Add(_bracket[_stage][i2]);
        }

        // Console.WriteLine("i2=" + i2 + " _bracket count=" + _bracket[_stage].Count);
        if (i2 == _bracket[_stage].Count-1)
        {
            _stage++;
            Console.WriteLine("Advancing to next stage!");
            return true;
        }

        return false;
    }

    public void PopulateBracket()
    {
        
    }
}