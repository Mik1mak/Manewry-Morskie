using CellLib;
using Microsoft.AspNetCore.SignalR;

namespace ManewryMorskie.Server
{
    public class ManewryMorskieHub : Hub
    {
        private readonly Rooms rooms;
        private readonly Dictionary<string, Client> clients;
        private readonly Client newClient;

        private Client Client => clients[Context.ConnectionId];

        public ManewryMorskieHub(Rooms rooms, Dictionary<string, Client> clients, Client newClient)
        {
            this.clients = clients;
            this.rooms = rooms;
            this.newClient = newClient;
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
            return Task.CompletedTask;
        }

        public Task ClickedLocation(CellLocation location)
        {
            Client.NetworkUserInterface.InvokeClickedLocation(location);
            return Task.CompletedTask;
        }

        public override Task OnConnectedAsync()
        {
            clients[Context.ConnectionId] = newClient;
            Client.SetCallerContext(Context);
            return Task.CompletedTask;
        }
    }
}
