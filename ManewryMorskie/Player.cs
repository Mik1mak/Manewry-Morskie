namespace ManewryMorskie
{

    public class Player
    {
        public Player(IUserInterface userInterface)
        {
            UserInterface = userInterface;
        }

        public IUserInterface UserInterface { get; protected set; }

        public int Color { get; set; }
        public string Name { get; set; } = nameof(Player);
        public Fleet Fleet {get; set;} = new Fleet();
    }
}
