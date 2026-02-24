using System.ComponentModel.Design;

namespace osuRefHelper
{
    internal class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public bool RollWinner { get; set; }
        public Player(string name)
        {
            Name = name;
            Score = 0;
            RollWinner = false;
        }
    }
    internal class Program
    {
        string filePath = "pool.txt";

        string[] pool;
        string tourneyAcronym;

        Player player1;
        Player player2;

        int bestOf;
        int roundsPlayed = 0;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        void Run()
        {
            ReadPool();

            Console.Write("Tourney Acronym: ");
            tourneyAcronym = Console.ReadLine();
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
                pool = File.ReadAllLines(filePath);
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

        void SetupLobby()
        {
            Console.Write("Player 1 (higher seed): ");
            string p1Name = Console.ReadLine();
            
            while(p1Name.Equals(""))
            {
                Console.Clear();
                Console.WriteLine("Player name cannot be empty!");
                Console.WriteLine("");
                Console.WriteLine("Press Enter to continue...");
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
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.Write("Player 2: ");
                p2Name = Console.ReadLine();
            }

            player2 = new Player(p2Name);

            Console.WriteLine("");

            Console.Write("Best of: ");
            while (!int.TryParse(Console.ReadLine(), out bestOf) || bestOf % 2 == 0)
            {
                Console.Clear();
                Console.WriteLine("Input must be a numeric ODD number!");
                Console.WriteLine("");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.Write("Best of: ");
            }

            Console.WriteLine("");

            Console.WriteLine("!mp set 0 3");
            Console.WriteLine("!mp invite " + player1.Name);
            Console.WriteLine("!mp invite " + player2.Name);

            Console.WriteLine("");

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        void AskRollWinner()
        {

            Console.WriteLine("Who won the roll?");
            Console.Write(player1.Name + "(1) or " + player2.Name + "(2)?");

            if(Console.ReadLine() == "1")
            {
                player1.RollWinner = true;
            }
            else if(Console.ReadLine() == "2")
            {
                player2.RollWinner = true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Wrong input!");
                Console.WriteLine("");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                AskRollWinner();
            }

            Console.WriteLine("");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();

        }

         void DisplayPool()
         {
            Console.Clear();

            Console.WriteLine("");

            Console.WriteLine("");

            Console.WriteLine("Remaining maps:");
            // Display all slots
            foreach (string line in pool)
            {
                string[] values = line.Split(':');
                Console.Write(values[0] + " ");
            }
            Console.WriteLine("\n");
         }

         void GetBans()
         {
            bool ban1Exists = false;
            bool ban2Exists = false;

            Console.Write("Ban 1: ");
            string ban1 = Console.ReadLine();

            foreach (string line in pool)
            {
                string[] values = line.Split(':');
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
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();

                DisplayPool();
                GetBans();
                return;
            }

            Console.Write("Ban 2: ");
            string ban2 = Console.ReadLine();

            foreach (string line in pool)
            {
                string[] values = line.Split(':');
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
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();

                DisplayPool();
                GetBans();
                return;
            }


            // Filter out lines that contain ban1 OR ban2 (case-insensitive)
            pool = pool.Where(line =>
                !line.Contains(ban1, StringComparison.OrdinalIgnoreCase) &&
                !line.Contains(ban2, StringComparison.OrdinalIgnoreCase)).ToArray();
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
                string[] values = line.Split(':');
                if (values[0].Equals(map, StringComparison.OrdinalIgnoreCase))
                {
                    mapExists = true;
                    break;
                }

            }

            if (!mapExists)
            {
                Console.Clear();
                Console.WriteLine("Wrong slot specified!");
                Console.WriteLine("");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();

                DisplayPool();
                Console.WriteLine("Press q to exit...");
                GetCommands(Console.ReadLine());

                return;
            }

            string[] mapid = pool.Where(line =>
                line.Contains(map, StringComparison.OrdinalIgnoreCase)).ToArray();

            Console.Clear();

            foreach (string line in mapid)
            {
                string slot = line.Split(':')[0];
                string id = line.Split(':')[1];

                Console.WriteLine(slot);

                Console.WriteLine("\n!mp map " + id);


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
                        !line.Contains(map, StringComparison.OrdinalIgnoreCase)).ToArray();

                roundsPlayed++;

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();

                Console.Clear();
                
                Console.WriteLine(player1.Name + " or " + player2.Name + "?");
                Console.Write("Round Winner: ");
                AskWinner(Console.ReadLine());
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
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
                    Console.WriteLine(player1.Name + " " + player1.Score + " - " + player2.Score + " " + player2.Name + " | " + "Winner: " + player1.Name);
                    Console.WriteLine("");
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                if (roundsPlayed % 2 != 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine(player1.Name + " " + player1.Score + " - " + player2.Score + " " + player2.Name + " | " + "Next Pick: " + player2.Name);
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(player1.Name + " " + player1.Score + " - " + player2.Score + " " + player2.Name + " | " + "Next Pick: " + player1.Name);
                    Console.WriteLine("");
                }
            }
            else
            if (winner.Equals(player2.Name, StringComparison.OrdinalIgnoreCase))
            {
                player2.Score++;

                if (player2.Score > bestOf / 2)
                {
                    Console.WriteLine("");
                    Console.WriteLine(player1.Name + " " + player1.Score + " - " + player2.Score + " " + player2.Name + " | " + "Winner: " + player2.Name);
                    Console.WriteLine("");
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                if (roundsPlayed % 2 != 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine(player1.Name + " " + player1.Score + " - " + player2.Score + " " + player2.Name + " | " + "Next Pick: " + player2.Name);
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(player1.Name + " " + player1.Score + " - " + player2.Score + " " + player2.Name + " | " + "Next Pick: " + player1.Name);
                    Console.WriteLine("");
                }

            }
            else
            {
                Console.Clear();

                Console.WriteLine(player1.Name + " or " + player2.Name + "?");
                Console.Write("Round Winner: ");
                AskWinner(Console.ReadLine());
            }
        }
    }
}
