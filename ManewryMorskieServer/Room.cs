namespace ManewryMorskie.Server
{
    public class Room
    {
        public bool IsWaitingForPlayers => clients.Count < 2;
        public bool ClientDisconnected { get; private set; }

        private ILogger? logger;
        private readonly List<Client> clients = new();
        public IReadOnlyList<Client> Clients => clients;

        private CancellationTokenSource? tokenSource;
        private Task? gameTask;


        public Room(ILogger? logger = null)
        {
            this.logger = logger;
        }

        public Room AddClient(Client client)
        {
            if (!IsWaitingForPlayers)
                throw new InvalidOperationException("Too many clients in Room!");

            clients.Add(client);
            client.Disconnecting += Client_Disconnecting;
            return this;
        }

        private async Task Client_Disconnecting()
        {
            logger?.LogInformation("Room Disconnected"); ;
            tokenSource?.Cancel();
            ClientDisconnected = true;

            foreach (Client client in clients)
                if (!client.IsDisconnected)
                    await client.Kick("Przeciwnik rozłączył się.");
        }

        public async Task RunGame()
        {
            Client player1 = clients[0];
            Client player2 = clients[1];
            player1.Color = 1;
            player1.Name = nameof(player1);
            player2.Name = nameof(player2);

            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(player1.CancellationToken, player2.CancellationToken);

            ManewryMorskieGame game = new(player1, player2, logger)
            {
                AsyncGame = false,
            };

            await player1.GameStarted();
            await player2.GameStarted();

            gameTask = game.Start(tokenSource.Token);
        }
    }
}
