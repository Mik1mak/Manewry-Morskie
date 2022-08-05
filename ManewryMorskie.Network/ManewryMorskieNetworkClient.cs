using CellLib;
using Microsoft.AspNetCore.SignalR.Client;

namespace ManewryMorskie.Network
{

    public class ManewryMorskieNetworkClient : IManewryMorskieClient
    {
        private (bool create, string? roomName, bool isRandomRoom) room;

        private readonly HubConnection connection;
        private readonly IUserInterface clientInterface;

        public event Func<string?, Task>? GameClosed;

        public event Func<Exception?, Task>? Reconnecting
        {
            add => connection.Reconnecting += value;
            remove => connection.Reconnecting -= value;
        }

        public event Func<string?, Task>? Reconnected
        {
            add => connection.Reconnected += value;
            remove => connection.Reconnected -= value;
        }

        public ManewryMorskieNetworkClient(IUserInterface ui, string url)
        {
            this.clientInterface = ui;

            connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            clientInterface.ClickedLocation += InvokeClickedLocation;
            clientInterface.ChoosenOptionId += InvokeChoosenOptionId;
            connection.Closed += Connection_Closed;

            connection.On<string, string[]>(nameof(IUserInterface.DisplayOptionsMenu), async (title, options) =>
                await clientInterface.DisplayOptionsMenu(title, options));

            connection.On<string, MessageType>(nameof(IUserInterface.DisplayMessage), async (msg, type) =>
                await clientInterface.DisplayMessage(msg, type));

            connection.On<CellLocation, string[]>(nameof(IUserInterface.DisplayContextOptionsMenu), async (location, options) =>
                await clientInterface.DisplayContextOptionsMenu(location, options));

            connection.On<IEnumerable<CellLocation>, MarkOptions>(nameof(IUserInterface.MarkCells), async (locations, markOption) =>
                await clientInterface.MarkCells(locations, markOption));

            connection.On<Move>(nameof(IUserInterface.ExecuteMove), async mv => 
                await clientInterface.ExecuteMove(mv));

            connection.On<CellLocation>(nameof(IUserInterface.TakeOffPawn), async l => 
                await clientInterface.TakeOffPawn(l));

            connection.On<CellLocation, int, bool, string>(nameof(IUserInterface.PlacePawn), async (l, c, b, d) =>
                await clientInterface.PlacePawn(l, c, b, d));
        }

        private async Task Connection_Closed(Exception? arg)
        {
            if(GameClosed != null)
                await GameClosed.Invoke("Gra została zakończona.");
        }

        private async void InvokeChoosenOptionId(object? sender, int e)
        {
            await connection.InvokeAsync(nameof(IUserInterface.ChoosenOptionId), e);
        }

        private async void InvokeClickedLocation(object? sender, CellLocation e)
        {
            await connection.InvokeAsync(nameof(IUserInterface.ClickedLocation), e);
        }

        public void SetRoom(bool create, string? roomName, bool isRandomRoom)
        {
            room = (create, roomName, isRandomRoom);
        }

        public async Task RunGame(CancellationToken ct = default)
        {
            await StopAsync();
            await connection.StartAsync(ct);

            await connection.InvokeAsync("PickRoom", room.create, room.roomName, room.isRandomRoom, ct);

            try
            {
                while (connection.State != HubConnectionState.Disconnected)
                {
                    await Task.Delay(800, ct);
                    ct.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                await StopAsync();
            }
        }

        private async ValueTask StopAsync()
        {
            if (connection.State != HubConnectionState.Disconnected)
                await connection.StopAsync(CancellationToken.None);
        }

        public async ValueTask DisposeAsync()
        {
            connection.Closed -= Connection_Closed;
            clientInterface.ClickedLocation -= InvokeClickedLocation;
            clientInterface.ChoosenOptionId -= InvokeChoosenOptionId;
            await connection.DisposeAsync();
        }
    }
}