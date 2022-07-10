namespace ManewryMorskie
{

    public class Player
    {
        public Player(IUserInterface userInterface)
        {
            UserInterface = userInterface;
        }

        public IUserInterface UserInterface { get; protected set; }

        public int Color { get; protected set; }
        public string Name { get; protected set; } = nameof(Player);
        public Fleet Fleet {get; protected set;} = new Fleet();
    }
}
