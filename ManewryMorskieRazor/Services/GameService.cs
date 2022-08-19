using ManewryMorskie.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace ManewryMorskieRazor
{
    public class GameService
    {
        private readonly BoardService boardService;
        private readonly UserInterface ui;
        private readonly DialogService dialogService;
        private readonly string serverUrl;

        private CancellationTokenSource? tokenSource;
        private IManewryMorskieClient? client;

        public GameService(UserInterface ui, BoardService boardService, DialogService dialogService, IConfiguration Configuration)
        {
            this.ui = ui;
            this.boardService = boardService;
            this.dialogService = dialogService;
            serverUrl = Configuration["ManewryMorskieServerUrl"];
        }

        public async ValueTask SetUpLocal()
        {
            await Clean();

            ManewryMorskieLocalClient localClient = new(ui);
            localClient.TurnChanged += Manewry_TurnChanged;
            client = localClient;
        }

        public async ValueTask SetUpOnline(bool create, string? roomName, bool randomRoom)
        {
            await Clean();

            ManewryMorskieNetworkClient networkClient = new(ui, serverUrl);
            networkClient.Reconnecting += NetworkClient_Reconnecting;
            networkClient.Reconnected += HideSplashScreen;
            networkClient.GameStarted += HideSplashScreen;
            networkClient.GameClosed += HideSplashScreen;
            dialogService.SplashScreenDismissed += AbortGame; ;
            networkClient.SetRoom(create, roomName, randomRoom);

            client = networkClient;
        }

        private async ValueTask Clean()
        {
            if (client is ManewryMorskieLocalClient localClient)
            {
                localClient.TurnChanged -= Manewry_TurnChanged;
            }
            else if(client is ManewryMorskieNetworkClient networkClient)
            {
                networkClient.Reconnecting -= NetworkClient_Reconnecting;
                networkClient.Reconnected -= HideSplashScreen;
                networkClient.GameStarted -= HideSplashScreen;
                networkClient.GameClosed -= HideSplashScreen;
                dialogService.SplashScreenDismissed -= HideSplashScreen;
            }

            if (client != null)
            {
                client.GameClosed -= Client_GameClosed;
                await client.DisposeAsync();
            }

            await ui.Clean();
        }

        public async Task RunGame()
        {
            if (client != null)
            {
                client.GameClosed += Client_GameClosed;

                tokenSource?.Cancel();
                await Task.Delay(5);
                tokenSource = new();

                await client.RunGame(tokenSource.Token);
            }
        }

        private async Task Client_GameClosed(string? arg)
        {
            client!.GameClosed -= Client_GameClosed;
            await client.DisposeAsync();
            await ui!.Clean();
        }

        private async void Manewry_TurnChanged(object? sender, int e)
        {
            await dialogService.DisplaySplashScreen(new("Kliknij, aby kontynuować", true));
        }

        private async Task NetworkClient_Reconnecting(Exception? arg)
        {
            await dialogService.DisplaySplashScreen(new("Utracono połączenie. Próbujemy najwiązać je ponownie.", false));
        }
        private async Task HideSplashScreen(string? arg)
        {
            await dialogService.DisplaySplashScreen(null);
        }
        private async Task HideSplashScreen()
        {
            dialogService.SplashScreenDismissed -= AbortGame;
            await dialogService.DisplaySplashScreen(null);
        }

        private async Task AbortGame()
        {
            await Clean();
        }
    }
}
