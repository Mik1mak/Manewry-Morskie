using Microsoft.AspNetCore.SignalR;

namespace ManewryMorskie.Server
{
    public class Client : Player
    {
        private readonly IHubContext<ManewryMorskieHub> hubContext;

        public string Id { get; private set; } = string.Empty;
        public CancellationToken CancellationToken { get; private set; }
        public NetworkUserInterface NetworkUserInterface => (NetworkUserInterface)this.UserInterface;

        public Client(IHubContext<ManewryMorskieHub> hubContext) 
            : base(new NetworkUserInterface(hubContext))
        {
            this.hubContext = hubContext;
        }

        public void SetCallerContext(HubCallerContext context)
        {
            NetworkUserInterface.ConnectionId = Id = context.ConnectionId;
            CancellationToken = context.ConnectionAborted;
        }

        public async Task Kick(string? message = null)
        {
            if(message != null)
                await UserInterface.DisplayMessage(message);

            await hubContext.Clients.Client(Id).SendAsync("Kick");
        }
    }
}
