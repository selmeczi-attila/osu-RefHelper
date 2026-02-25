using System.Collections.Generic;

namespace osuRefHelper
{
    internal class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public bool RollWinner { get; set; }
        public bool firstPick { get; set; }
        public bool firstBan { get; set; }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            RollWinner = false;
            firstPick = false;
            firstBan = false;
        }
    }
    internal class Program
    {

        // TODO: ADD CONFIG FILE INTEGRATION!!! ASAP, more null/invalid input/pool txt correctness check and all other checks ig

        string filePath = "pool.txt";

        List<string> pool;
        string tourneyAcronymAndRound;

        Player player1;
        Player player2;

        int bestOf;
        int roundsPlayed = 0;

        bool tiebreaker = false;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        void Run()
        {
            ReadPool();
            Console.WriteLine("Example: HOL Group Stage");
            Console.Write("Tourney Acronym + Round: ");
            tourneyAcronymAndRound = Console.ReadLine();
            Console.Clear();

            SetupLobby();

            AskRollWinner();

            DisplayPool();

            GetBans();

            DisplayPool();

            Console.WriteLine("Press q to exit...");
            GetCommands(Console.ReadLine());
        }

        void ReadPool()
        {
            if (File.Exists(filePath))
            {
                pool = File.ReadAllLines(filePath).ToList();
            }
            else
            {
                Console.WriteLine("pool.txt doesn't exist.");
                Console.WriteLine("");
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        // TODO: Dont let 2 players have the same name, run whole function from the start so p1 p2 gets requested again
        void SetupLobby()
        {
            Console.Write("Player 1 (higher seed): ");
            string p1Name = Console.ReadLine();

            while (p1Name.Equals(""))
            {
                Console.Clear();
                Console.WriteLine("Player name cannot be empty!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.Write("Player 1 (higher seed): ");
                p1Name = Console.ReadLine();
            }

            player1 = new Player(p1Name);

            Console.Write("Player 2: ");
            string p2Name = Console.ReadLine();

            while (p2Name.Equals(""))
            {
                Console.Clear();
                Console.WriteLine("Player name cannot be empty!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.Write("Player 2: ");
                p2Name = Console.ReadLine();
            }

            player2 = new Player(p2Name);

            //Console.Title = (player1.Name + " vs " + player2.Name);

            Console.WriteLine("");

            Console.Write("Best of: ");
            while (!int.TryParse(Console.ReadLine(), out bestOf) || bestOf % 2 == 0)
            {
                Console.Clear();
                Console.WriteLine("Input must be a numeric ODD number!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.Write("Best of: ");
            }

            Console.WriteLine("");

            // remove this after config file rounds addition
            Console.WriteLine("skip if not group stage");
            Console.Write("Enter group stage lobby letter: ");
            string groupLetter = Console.ReadLine();

            Console.WriteLine("");

            // TODO: config file specify round and make this not fixed round with acronym
            // HOL Group Stage: defii vs geczy (GROUP A)
            if (string.IsNullOrEmpty(groupLetter)){
                Console.WriteLine($"!mp make {tourneyAcronymAndRound}: {player1.Name} vs {player2.Name}");
            }
            else
            {
                Console.WriteLine($"!mp make {tourneyAcronymAndRound}: {player1.Name} vs {player2.Name} (GROUP {groupLetter})");
            }

            Console.WriteLine("!mp set 0 3");
            Console.WriteLine($"!mp invite {player1.Name}");
            Console.WriteLine($"!mp invite {player2.Name}");

            Console.WriteLine("");

            Console.Write("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        void AskRollWinner()
        {
            Console.WriteLine("Who won the roll?");
            Console.Write($"{player1.Name}(1) or {player2.Name}(2)? ");

            string rollWinnerInput = Console.ReadLine();

            if (rollWinnerInput == "1")
            {
                player1.RollWinner = true;
            }
            else if (rollWinnerInput == "2")
            {
                player2.RollWinner = true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Wrong input!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                AskRollWinner();
                return;
            }

            Console.WriteLine("");

            Console.WriteLine("First pick or ban?");
            Console.Write("Pick(1) or Ban(2)? ");
            string firstInput = Console.ReadLine();

            while(firstInput != "1" && firstInput != "2")
            {
                Console.Clear();
                Console.WriteLine("Wrong input!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.WriteLine("First pick or ban?");
                Console.Write("Pick(1) or Ban(2)? ");
                firstInput = Console.ReadLine();
            }

            //todo:             
            if(rollWinnerInput == "1")
            {
                if(firstInput == "1")
                {
                    player1.firstPick = true;
                    player2.firstBan = true;
                }else if (firstInput == "2")
                {
                    player1.firstBan = true;
                    player2.firstPick = true;
                }
            }

            if(rollWinnerInput == "2")
            {
                if (firstInput == "1")
                {
                    player2.firstPick = true;
                    player1.firstBan = true;
                }
                else if (firstInput == "2")
                {
                    player2.firstBan = true;
                    player1.firstPick = true;
                }
            }

            Console.Clear();
        }

         void DisplayPool()
         {
            Console.Clear();
            if(roundsPlayed == 0)
            {
                Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name}");
            }
            else
            {
                if (player1.RollWinner && player1.firstPick)
                {
                    if (roundsPlayed % 2 != 0)
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player2.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player1.Name}");
                    }
                }
                else if (player1.RollWinner && !player1.firstPick)
                {
                    if (roundsPlayed % 2 != 0)
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player1.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player2.Name}");
                    }
                }
                else if (player2.RollWinner && player2.firstPick)
                {
                    if (roundsPlayed % 2 != 0)
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player1.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player2.Name}");
                    }
                }
                else if (player2.RollWinner && !player2.firstPick)
                {
                    if (roundsPlayed % 2 != 0)
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player2.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Next Pick: {player1.Name}");
                    }
                }
            }

            Console.WriteLine("");

            if(player1.Score == (bestOf - 1) / 2 && player2.Score == (bestOf - 1) / 2)
            {
                tiebreaker = true;
                foreach (string line in pool)
                {
                    pool = pool.Where(line => line.Contains("tb", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                Console.WriteLine("Remaining maps:");
                foreach (string line in pool)
                {
                    List<string> values = line.Split(':').ToList();
                    Console.Write($"{values[0]} ");
                }
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("Remaining maps:");
                foreach (string line in pool)
                {
                    List<string> values = line.Split(':').ToList();
                    if (!values[0].Contains("tb", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write($"{values[0]} ");
                    }
                }
                Console.WriteLine("");
            }
         }

         void GetBans()
         {
            string ban1 = "";
            string ban2 = "";

            bool ban1Exists = false;
            bool ban2Exists = false;

            if (player1.firstBan)
            {
                Console.Write($"{player1.Name}'s Ban: ");
                ban1 = Console.ReadLine();

                foreach (string line in pool)
                {
                    List<string> values = line.Split(':').ToList();
                    if (values[0].Equals(ban1, StringComparison.OrdinalIgnoreCase))
                    {
                        ban1Exists = true;
                        break;
                    }
                }

                if (!ban1Exists)
                {
                    Console.Clear();
                    Console.WriteLine("Wrong slot for ban specified!");
                    Console.WriteLine("");
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                    Console.Clear();

                    DisplayPool();

                    Console.WriteLine("");

                    GetBans();
                    return;
                }

                Console.Write($"{player2.Name}'s Ban: ");

                ban2 = Console.ReadLine();

                foreach (string line in pool)
                {
                    List<string> values = line.Split(':').ToList();
                    if (values[0].Equals(ban2, StringComparison.OrdinalIgnoreCase))
                    {
                        ban2Exists = true;
                        break;
                    }
                }

                if (!ban2Exists)
                {
                    Console.Clear();
                    Console.WriteLine("Wrong slot for ban specified!");
                    Console.WriteLine("");
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                    Console.Clear();

                    DisplayPool();
                    GetBans();
                    return;
                }
            }else if (player2.firstBan)
            {
                Console.Write($"{player2.Name}'s Ban: ");

                ban2 = Console.ReadLine();

                foreach (string line in pool)
                {
                    List<string> values = line.Split(':').ToList();
                    if (values[0].Equals(ban2, StringComparison.OrdinalIgnoreCase))
                    {
                        ban2Exists = true;
                        break;
                    }
                }

                if (!ban2Exists)
                {
                    Console.Clear();
                    Console.WriteLine("Wrong slot for ban specified!");
                    Console.WriteLine("");
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                    Console.Clear();

                    DisplayPool();
                    GetBans();
                    return;
                }

                Console.Write($"{player1.Name}'s Ban: ");
                ban1 = Console.ReadLine();

                foreach (string line in pool)
                {
                    List<string> values = line.Split(':').ToList();
                    if (values[0].Equals(ban1, StringComparison.OrdinalIgnoreCase))
                    {
                        ban1Exists = true;
                        break;
                    }
                }

                if (!ban1Exists)
                {
                    Console.Clear();
                    Console.WriteLine("Wrong slot for ban specified!");
                    Console.WriteLine("");
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                    Console.Clear();

                    DisplayPool();
                    GetBans();
                    return;
                }
            }

            pool = pool.Where(line =>
                !line.Contains(ban1, StringComparison.OrdinalIgnoreCase) &&
                !line.Contains(ban2, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // TODO: when picks are getting inputted with readline, show who is picking like {rise's pick: [map]}
         void GetCommands(string map)
        {
            bool mapExists = false;

            if (map == "q")
            {
                Environment.Exit(0);
            }

            foreach (string line in pool)
            {
                List<string> values = line.Split(':').ToList();
                if (values[0].Equals(map, StringComparison.OrdinalIgnoreCase))
                {
                    if (!tiebreaker && values[0].Contains("tb", StringComparison.OrdinalIgnoreCase))
                    {
                        mapExists = false;
                    }
                    else
                    {
                        mapExists = true;
                    }
                    break;
                }

            }

            if (!mapExists)
            {
                Console.Clear();
                Console.WriteLine("Wrong slot specified!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();

                DisplayPool();
                Console.WriteLine("Press q to exit...");
                GetCommands(Console.ReadLine());

                return;
            }

            List<string> mapid = pool.Where(line =>
                line.Contains(map, StringComparison.OrdinalIgnoreCase)).ToList();

            Console.Clear();

            foreach (string line in mapid)
            {
                List<string> lineParts = line.Split(':').ToList();
                string slot = lineParts[0];
                string id = lineParts[1];

                Console.WriteLine(slot);

                Console.WriteLine("");

                Console.WriteLine($"!mp map {id}");

                if(line.Contains("nm", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("!mp mods nf");
                } else
                if (line.Contains("hd", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("!mp mods nf hd");
                } else
                if (line.Contains("hr", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("!mp mods nf hr");
                } else
                if (line.Contains("dt", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("!mp mods nf dt");
                } else
                if (line.Contains("tb", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("!mp mods nf");
                }

                Console.WriteLine("");

                pool = pool.Where(line =>
                        !line.Contains(map, StringComparison.OrdinalIgnoreCase)).ToList();

                roundsPlayed++;

                Console.Write("Press Enter to continue...");
                Console.ReadLine();

                Console.Clear();
                
                Console.WriteLine($"{player1.Name} or {player2.Name}?");
                Console.Write("Round Winner: ");
                AskWinner(Console.ReadLine());
            }

            Console.Clear();

            DisplayPool();
            Console.WriteLine("Press q to exit...");
            GetCommands(Console.ReadLine());
         }

         void AskWinner(string winner)
         {
            if (winner.Equals(player1.Name, StringComparison.OrdinalIgnoreCase))
            {
                player1.Score++;

                if (player1.Score > bestOf / 2)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Winner: {player1.Name}");
                    Console.WriteLine("");
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            else
            if (winner.Equals(player2.Name, StringComparison.OrdinalIgnoreCase))
            {
                player2.Score++;

                if (player2.Score > bestOf / 2)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"{player1.Name} {player1.Score} - {player2.Score} {player2.Name} | Winner: {player2.Name}");
                    Console.WriteLine("");
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.Clear();

                Console.WriteLine($"{player1.Name} or {player2.Name}?");
                Console.Write("Round Winner: ");
                AskWinner(Console.ReadLine());
            }
        }
    }
}
