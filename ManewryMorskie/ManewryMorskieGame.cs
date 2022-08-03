using CellLib;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{

    public class ManewryMorskieGame : IAsyncDisposable
    {
        private InternationalWaterManager internationalWaterManager;
        private StandardMap map;
        private GameEndDetector endDetector;
        private PlayerManager playerManager;
        private MoveExecutor executor;

        private readonly TurnCounter turnManager = new();

        public bool AsyncGame { get; set; } = false;

        public ManewryMorskieGame(Player player1, Player player2)
        {
            playerManager = new PlayerManager(turnManager, player1, player2);

            map = StandardMap.DefaultMap(playerManager);

            internationalWaterManager = new InternationalWaterManager(map);

            turnManager.TurnChanging += TurnCounter_TurnChanging;
            internationalWaterManager.InternedUnit += InternationalWaterManager_InternedUnit;

            endDetector = new GameEndDetector(map, turnManager, playerManager);
            endDetector.GameEnded += EndDetector_GameEnded;

            executor = new(map, playerManager);
        }

        private async void EndDetector_GameEnded(object sender, GameEnd e)
        {
            switch(e.GameEndReason)
            {
                case GameEndReason.DestroyedOkretDesantowy:
                    await playerManager.WriteToPlayers(e.Winner!,
                        msgToCurrent: "Zwycięstwo! Zniszczyłeś okręt desantowy przeciwnika!",
                        msgToOthers: "Porażka! Przeciwnik zniszczył Twój okręt desantowy!");
                    break;
                case GameEndReason.DestroyedOkretyRakietowe:
                    await playerManager.WriteToPlayers(e.Winner!,
                        msgToCurrent: "Zwycięstwo! Zniszczyłeś okręty rakietowe przeciwnika utrzymując obronę portu!",
                        msgToOthers: "Porażka! Przeciwnik zniszczył Twóje okręty rakietowe utrzymując obronę portu!");
                    break;
                case GameEndReason.OkretDesantowyReachedEnemyField:
                    await playerManager.WriteToPlayers(e.Winner!,
                        msgToCurrent: "Zwycięstwo! Twój okręt desantowy wpłynął do portu przeciwnika!",
                        msgToOthers: "Porażka! Okręt desantowy przeciwnika wpłynął do Twojego portu!");
                    break;
            }
        }

        private void TurnCounter_TurnChanging(object sender, int e)
        {
            internationalWaterManager.Iterate();
        }

        private async void InternationalWaterManager_InternedUnit(object sender, Unit e)
        {
            foreach (CellLocation location in internationalWaterManager.InternationalWaters)
            {
                MapField field = map[location];

                if (field.Unit == e)
                {
                    Player player = playerManager.First(p => p.Fleet.Units.Contains(e));

                    foreach (IUserInterface ui in playerManager.UniqueInferfaces)
                    {
                        await ui.TakeOffPawn(location);
                        await ui.DisplayMessage($"Jednostka {e} została internowana, ponieważ przebywała przez " +
                            $"{internationalWaterManager.TurnsOnInternationalWaterLimit} tur na wodach międzynarodowych!");
                    }
                    player.Fleet.Destroy(e);

                    field.Unit = null;
                    return;
                }
            }   
        }

        public async Task Start(CancellationToken token)
        {
            using (IPlacingManager currentPlacingMgr = new PawnsPlacingManager(map, playerManager, playerManager.CurrentPlayer))
            {
                Task currentPlayerPlacingTask = currentPlacingMgr.PlacePawns(token);
                token.ThrowIfCancellationRequested();

                if (!AsyncGame)
                    await Task.WhenAll(currentPlayerPlacingTask);

                using (IPlacingManager opositePlacingMgr =
                    new PawnsPlacingManager(map, playerManager, playerManager.GetOpositePlayer()))
                {
                    Task opositePlayerPlacingTask = opositePlacingMgr.PlacePawns(token);
                    token.ThrowIfCancellationRequested();
                    await Task.WhenAll(currentPlayerPlacingTask, opositePlayerPlacingTask);
                }
            }

            using TurnManager turnMgr = new(map, playerManager, internationalWaterManager);

            while (!endDetector.GameIsEnded)
            {
                Move move = await turnMgr.MakeMove(token);
                await executor.Execute(move);
                turnManager.NextTurn();
                token.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask DisposeAsync()
        {
            foreach (IUserInterface ui in playerManager.UniqueInferfaces)
            {
                foreach (CellLocation l in map.Keys)
                    if (map[l].Unit is not null)
                        await ui.TakeOffPawn(l);

                await ui.MarkCells(map.Keys, MarkOptions.None);
                await ui.DisplayMessage(string.Empty, MessageType.Empty);
            }
        }
    }
}
