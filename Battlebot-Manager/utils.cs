using Battlebot_Manager;

public static class StringExt
{
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength); 
    }
}

public static class ListExt
{
    public static T RandomPick<T>(this List<T> list)
    {
        return list[Random.Shared.Next(0, list.Count)];
    }
    
    public static T RandomPop<T>(this List<T> list)
    {
        int i = Random.Shared.Next(0, list.Count);
        T var = list[i];
        list.RemoveAt(i);
        return var;
    }
}

public static class Utils
{
    // asks the user to choose from a list of numbered options, 
    public static int Prompt(string[] prompts)
    {
        while (true)
        {
            for (int i = 0; i < prompts.Length; i++)
            {
                Console.WriteLine($"{i+1}> {prompts[i]}");
            }
            Console.WriteLine();
            Console.Write("?> ");
            string response = "";
            response = Console.ReadLine();
            int rint;
            if (int.TryParse(response, out rint) && rint > 0 && rint <= prompts.Length)
            {
                return rint;
            }
            Console.WriteLine();
            Console.WriteLine("Not a valid response!");
            Console.WriteLine();
        }
    }

    public static Robot Prompt(Robot[] robots)
    {
        List<string> prompts = new List<string>();
        foreach (Robot r in robots)
        {
            prompts.Add(r.Name);
        }
        return robots[Prompt(prompts.ToArray()) - 1];
    }
    
    // asks the user to respond to prompt with Y/N, returns true for Y and false for N
    public static bool PromptYN(string prompt)
    {
        while (true)
        {
            if (prompt != "")
            {
                Console.WriteLine(prompt);
            }
            Console.Write("Y/N?> ");
            
            string response = "";
            response = Console.ReadLine();
            if (response.ToLower() == "y")
            {
                return true;
            }

            if (response.ToLower() == "n")
            {
                return false;
            }
            Console.WriteLine();
            Console.WriteLine("Not a valid response!");
            Console.WriteLine();
        }
    }
}