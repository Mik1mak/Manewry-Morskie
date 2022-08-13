using CellLib;
using Microsoft.AspNetCore.SignalR;

namespace ManewryMorskie.Server
{
    public class NetworkUserInterface : IUserInterface
    {
        private readonly IHubContext<ManewryMorskieHub> context;
        public string? ConnectionId { get; set; }

        private IClientProxy Client
        {
            get
            {
                if(ConnectionId == null)
                    throw new InvalidOperationException();

                return context.Clients.Client(ConnectionId);
            }
        }

        public event EventHandler<CellLocation>? ClickedLocation;
        public event EventHandler<int>? ChoosenOptionId;        

        public NetworkUserInterface(IHubContext<ManewryMorskieHub> context)
        {
            this.context = context;
        }

        public void InvokeClickedLocation(CellLocation location) => ClickedLocation?.Invoke(this, location);
        public void InvokeChoosenOptionId(int optionId) => ChoosenOptionId?.Invoke(this, optionId);

        public async Task DisplayContextOptionsMenu(CellLocation location, params string[] options)
        {
            await Client.SendAsync(nameof(IUserInterface.DisplayContextOptionsMenu), location, options);
        }

        public async Task DisplayMessage(string message, MessageType msgType = MessageType.Standard)
        {
            await Client.SendAsync(nameof(IUserInterface.DisplayMessage), message, msgType);
        }

        public async Task DisplayOptionsMenu(string title, params string[] options)
        {
            await Client.SendAsync(nameof(IUserInterface.DisplayOptionsMenu), title, options);
        }

        public async Task ExecuteMove(Move mv)
        {
            await Client.SendAsync(nameof(IUserInterface.ExecuteMove), mv);
        }

        public async Task MarkCells(IEnumerable<CellLocation> cells, MarkOptions mode)
        {
            await Client.SendAsync(nameof(IUserInterface.MarkCells), cells, mode);
        }

        public async Task PlacePawn(CellLocation location, int playerColor, bool battery = false, string pawnDescription = "")
        {
            await Client.SendAsync(nameof(IUserInterface.PlacePawn), location, playerColor, battery, pawnDescription);
        }

        public async Task TakeOffPawn(CellLocation location)
        {
            await Client.SendAsync(nameof(IUserInterface.TakeOffPawn), location);
        }
    }
}
