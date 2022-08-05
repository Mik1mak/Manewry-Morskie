using CellLib;

namespace ManewryMorskie.Server
{
    public class Room
    {
        public string? Name { get; set; }

        public bool IsRandomRoom { get; set; }


    }

    public class Client : Player
    {
        public string Id { get; private set; }
        public CancellationToken CancellationToken { get; set; }

        public Client(string id, CancellationToken token) : base(new NetworkUserInterface(id))
        {
            Id = id;
            CancellationToken = token;
        }
    }

    public class NetworkUserInterface : IUserInterface
    {
        private readonly string playerId;

        public event EventHandler<CellLocation>? ClickedLocation;
        public event EventHandler<int>? ChoosenOptionId;


        public NetworkUserInterface(string playerId)
        {
            this.playerId = playerId;
        }

        public Task DisplayContextOptionsMenu(CellLocation location, params string[] options)
        {
            throw new NotImplementedException();
        }

        public Task DisplayMessage(string message, MessageType msgType = MessageType.Standard)
        {
            throw new NotImplementedException();
        }

        public Task DisplayOptionsMenu(string title, params string[] options)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteMove(Move mv)
        {
            throw new NotImplementedException();
        }

        public Task MarkCells(IEnumerable<CellLocation> cells, MarkOptions mode)
        {
            throw new NotImplementedException();
        }

        public Task PlacePawn(CellLocation location, int playerColor, bool battery = false, string pawnDescription = "")
        {
            throw new NotImplementedException();
        }

        public Task TakeOffPawn(CellLocation location)
        {
            throw new NotImplementedException();
        }
    }
}
