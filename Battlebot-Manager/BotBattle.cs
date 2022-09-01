using System;
namespace Battlebot_Manager;


public static class BotBattle
{
    public static bool Fight(Robot bot1, Robot bot2)
    {
        Console.WriteLine($"In the blue corner: {bot1.Name} of {bot1.Team.Name}");
        Console.WriteLine($"In the red corner: {bot2.Name} of {bot2.Team.Name}");
        Console.WriteLine();
        //Thread.Sleep(1000);

        //int odds = (int)MathF.Round(10 * (enemyBot.Strongness / (float)(playerBot.Strongness + enemyBot.Strongness)) + 2);
        int b1Adv = bot1.Strongness;
        int b2Adv = bot2.Strongness;

        Console.WriteLine($"{bot1.Name}'s power level is {bot1.Strongness}");
        Console.WriteLine($"{bot2.Name}'s power level is {bot2.Strongness}");
        if (bot1.Weight > bot2.Weight)
        {
            Console.WriteLine($"{bot1.Name} has a weight advantage of {bot1.Weight - bot2.Weight}");
            b1Adv += bot1.Weight - bot2.Weight;
        }

        if (bot2.Weight > bot1.Weight)
        {
            Console.WriteLine($"{bot2.Name} has a weight advantage of {bot2.Weight - bot1.Weight}");
            b2Adv += bot2.Weight - bot1.Weight;
        }
        Console.WriteLine("press enter to fight...");
        Console.ReadLine();

        while (true)
        {
            int b1r1 = Random.Shared.Next(1, 7);
            int b1r2 = Random.Shared.Next(1, 7);
            int b1Total = b1r1 + b1r2 + b1Adv;
            int b2r1 = Random.Shared.Next(1, 7);
            int b2r2 = Random.Shared.Next(1, 7);
            int b2Total = b2r1 + b2r2 + b2Adv;
        
            Console.WriteLine($"{bot1.Name} rolled:");
            Console.WriteLine($"  {b1Total} from ({b1r1}, {b1r2})+{b1Adv}");
            Console.WriteLine($"{bot2.Name} rolled:");
            Console.WriteLine($"  {b2Total} from ({b2r1}, {b2r2})+{b2Adv}");
            Console.WriteLine();
            
            
            if (b1Total > b2Total)
            {
                Console.WriteLine($"{bot1.Name} wins!");
                Console.WriteLine();
                Console.WriteLine("press enter to continue...");
                Console.ReadLine();
                return true;
            }

            if (b2Total > b1Total)
            {
                Console.WriteLine($"{bot2.Name} wins!");
                Console.WriteLine();
                Console.WriteLine("press enter to continue...");
                Console.ReadLine();
                return false;
            }
            
            Console.WriteLine("Neither bot got the upper hand!");
            Console.WriteLine();
            Thread.Sleep(500);
        }
    }
}