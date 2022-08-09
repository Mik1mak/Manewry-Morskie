using Microsoft.AspNetCore.SignalR;

namespace ManewryMorskie.Server
{
    public class ManewryMorskieHub : Hub
    {
        Dictionary<string, Client> clients = new();

        public override Task OnConnectedAsync()
        {

            return base.OnConnectedAsync();
        }
    }
}
