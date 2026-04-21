//This class will be filled when finishing the team support. For the separation of them.
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