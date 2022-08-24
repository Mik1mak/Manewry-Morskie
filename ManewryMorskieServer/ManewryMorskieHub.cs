using CellLib;
using Microsoft.AspNetCore.SignalR;

namespace ManewryMorskie.Server
{
    public class ManewryMorskieHub : Hub
    {
        private readonly Rooms rooms;
        private readonly Client newClient;
        private readonly ILogger<ManewryMorskieHub> logger;

        private Client Client => (Context.Items[nameof(Client)] as Client)!;

        public ManewryMorskieHub(Rooms rooms, Client newClient, ILogger<ManewryMorskieHub> logger)
        {
            this.rooms = rooms;
            this.newClient = newClient;
            this.logger = logger;
        }

        public async Task CreateRoom(string? name)
        {
            if (name == null)
                await rooms.CreateRandomRoom(Groups, Client);
            else
                await rooms.CreateRoom(name, Groups, Client);
        }

        public async Task JoinToRoom(string? name)
        {
            if (name == null)
                await rooms.JoinToRandomRoom(Groups, Client);
            else
                await rooms.JoinToRoom(name, Groups, Client);
        }
   
        public Task ChoosenOptionId(int optionId)
        {
            Client.NetworkUserInterface.InvokeChoosenOptionId(optionId);
            logger.LogDebug("Client {clientId} choosed {optionId} loccation", Client.Id, optionId);
            return Task.CompletedTask;
        }

        public Task ClickedLocation(CellLocation location)
        {
            Client.NetworkUserInterface.InvokeClickedLocation(location);
            logger.LogDebug("Client {clientId} clicked {location} loccation", Client.Id, location);
            return Task.CompletedTask;
        }

        public override Task OnConnectedAsync()
        {
            Context.Items.Add(nameof(Client), newClient);
            Client.SetCallerContext(Context);
            logger.LogInformation("Client {clientId} connected", Client.Id);
            return Task.CompletedTask;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
                logger.LogError("Client {clientId} disconnected with exeption {exception}", Client.Id, exception!.Message);
            else
                logger.LogInformation("Client {clientId} disconnected", Client.Id);

            await Client.Disconnect();
            rooms.ClearDisconnectedRooms();
        }
    }
}
