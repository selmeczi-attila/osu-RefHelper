namespace osuRefHelper
{
    internal class Program
    {

        // TODO: ADD CONFIG FILE INTEGRATION!!! ASAP, more null/invalid input/pool txt correctness check and all other checks ig
        // refactor everything for nicer looks, more efficient code, less junk

        string configFilePath = "config.txt";
        string filePath = "pool.txt";
        string csvnamePath = "players.csv";

        List<string> pool;
        List<string> configLines;
        string tourneyAcronymAndRound;

        string acronym;
        int numberofbans;
        List<string> playerNames;

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
            ReadConfig();

            ReadPool();

            RoundSetup();

            SetupLobby();

            AskRollWinner();

            DisplayPool();

            GetBans();

            DisplayPool();

            Console.WriteLine("Press q to exit...");
            ReadPicks();
        }

        void WriteConfig()
        {
            Console.WriteLine("config.txt is missing. Create a config.txt file with the following content:");
            Console.Write("TeamSize:");
            string teamSize = Console.ReadLine();
            if(teamSize == "0")
            {
                Console.Clear();
                Console.WriteLine("Team size cannot be 0!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                WriteConfig();
                return;
            }

            Console.Write("Acronym:");
            string acronym = Console.ReadLine();
            
            if(teamSize == "1")
            {
                Console.Write("Sheet url(Players):");
                string players = Console.ReadLine();

                configLines = new List<string>
                {
                    $"TeamSize;{teamSize}",
                    $"Acronym;{acronym}",
                    $"Sheet url(Players);{players}",
                };
                
                File.WriteAllLines(configFilePath, configLines);
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            //WIP TEAM CONFIG, not working yet.
            else
            {
                Console.WriteLine("You chose team tournament style, please make a teams.txt file with teamnames and players in each team after the config.");
                configLines = new List<string>
            {
                $"TeamSize;{teamSize}",
                $"Acronym;{acronym}"
            };
                File.WriteAllLines(configFilePath, configLines);
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                ReadConfig();
            }      
        }

        void ReadConfig()
        {   //Maybe this will be moved to WriteConfig.
            if (File.Exists(configFilePath))
            {
                configLines = File.ReadAllLines(configFilePath).ToList();
                playerNames = File.ReadAllLines(csvnamePath).ToList();

                
                string csvUrl = "";
                foreach (string line in configLines)
                {
                    List<string> url = line.Split(';').ToList();
                    if (url[0].Equals("Sheet url(Players)", StringComparison.OrdinalIgnoreCase))
                    {
                        csvUrl = url[1].Trim();
                        break;
                    }
                }
                using var client = new HttpClient();
                string csvContent = client.GetStringAsync(csvUrl).Result;
                
                File.WriteAllTextAsync(csvnamePath, csvContent).Wait();

            }
            else
            {
                WriteConfig();
            }
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
    
        void RoundSetup()
        {
            foreach (string line in configLines)
            {
                List<string> acro = line.Split(';').ToList();
                if (acro[0].Equals("Acronym", StringComparison.OrdinalIgnoreCase))
                {
                    acronym = acro[1].Trim();
                    break;
                }
            }
            Console.WriteLine("Example: Group Stage");
            Console.Write("Tourney Round: ");
            tourneyAcronymAndRound = $"{acronym} {Console.ReadLine()}";
            Console.Write("Number of bans: ");
            numberofbans = Convert.ToInt32(Console.ReadLine());
            if(numberofbans != 0 && numberofbans <= 2)
            {
                 Console.Clear();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Number of bans must be between 1 and 2!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                RoundSetup();
                return;
            }

            if(!int.TryParse(numberofbans.ToString(), out int result))
            {
                Console.Clear();
                Console.WriteLine("Number of bans must be a numeric value!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                RoundSetup();
                return;
            }

            playerNames = playerNames.Select(name => name.Trim('"')).ToList();
            }

        void SetupLobby()
        {   
            Console.Write("Player 1 (higher seed): ");
            string p1Name = Console.ReadLine();
            if(playerNames.Contains(p1Name))
            {  
                player1 = new Player(p1Name);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Player name not found!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                SetupLobby();
            }

            while (p1Name.Equals(""))
            {
                Console.Clear();
                Console.WriteLine("Player name cannot be empty!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                SetupLobby();
            }

            
            Console.Write("Player 2: ");
            string p2Name = Console.ReadLine();
            if(playerNames.Contains(p2Name))
            {  
                player2 = new Player(p2Name);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Player name not found!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                SetupLobby();
            }

            while (p2Name.Equals(""))
            {
                Console.Clear();
                Console.WriteLine("Player name cannot be empty!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                SetupLobby();
            }

            if(p1Name.Equals(p2Name, StringComparison.OrdinalIgnoreCase))
            {
                Console.Clear();
                Console.WriteLine("Player 1 and Player 2 cannot have the same name!");
                Console.WriteLine("");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                SetupLobby();
                return;
            }
            else
            {
                 player2 = new Player(p2Name);
            }
           

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
            if(numberofbans == 1)
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
        }

        void ReadPicks()
        {   
            int totalScore = player1.Score + player2.Score;
                Player picker = (totalScore % 2 == 0) ? (player1.firstPick == true ? player1 : player2) : (player1.firstPick == true ? player2 : player1);   
                    Console.Write($"{picker.Name}'s Pick: ");
                    GetCommands(Console.ReadLine());
            
        }

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
                if (!long.TryParse(lineParts[1], out long id))
                {
                    Console.WriteLine($"Invalid map id in slot {slot}: '{lineParts[1]}' is not numeric. Please check pool.txt.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

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
            ReadPicks();
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
