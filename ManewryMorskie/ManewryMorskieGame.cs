using CellLib;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{

    public class ManewryMorskieGame
    {
        public event EventHandler<int>? TurnChanged;

        private InternationalWaterManager internationalWaterManager;
        private StandardMap map;
        private GameEndDetector endDetector;
        private PlayerManager playerManager;
        private MoveExecutor executor;

        private readonly ILogger? logger;

        private readonly TurnCounter turnManager = new();

        public bool AsyncGame { get; set; } = false;

        public ManewryMorskieGame(Player player1, Player player2, ILogger? logger = null)
        {
            this.logger = logger;
            playerManager = new PlayerManager(turnManager, player1, player2);

            map = StandardMap.DefaultMap(playerManager);

            internationalWaterManager = new InternationalWaterManager(map);

            turnManager.TurnChanging += TurnCounter_TurnChanging;
            turnManager.TurnChanged += TurnManager_TurnChanged;
            internationalWaterManager.InternedUnit += InternationalWaterManager_InternedUnit;

            endDetector = new GameEndDetector(map, turnManager, playerManager);
            endDetector.GameEnded += EndDetector_GameEnded;

            executor = new(map, playerManager);
        }

        private void TurnManager_TurnChanged(object sender, int e) => TurnChanged?.Invoke(this, playerManager.CurrentPlayer.Color);

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
                            "4 tury na wodach międzynarodowych!");
                    }
                    player.Fleet.Destroy(e);

                    field.Unit = null;
                    return;
                }
            }   
        }

        public async Task Start(CancellationToken token)
        {
            logger?.LogInformation("Game Started.");
            using (IPlacingManager currentPlacingMgr = new PawnsPlacingManager(map, playerManager, playerManager.CurrentPlayer, logger))
            {
                Task currentPlayerPlacingTask = currentPlacingMgr.PlacePawns(token);
                token.ThrowIfCancellationRequested();

                if (!AsyncGame)
                {
                    await Task.WhenAll(currentPlayerPlacingTask);
                    logger?.LogInformation("First Player setup pawns in async game.");
                    TurnChanged?.Invoke(this, playerManager.GetOpositePlayer().Color);
                }

                using (IPlacingManager opositePlacingMgr =
                    new PawnsPlacingManager(map, playerManager, playerManager.GetOpositePlayer(), logger))
                {
                    Task opositePlayerPlacingTask = opositePlacingMgr.PlacePawns(token);
                    token.ThrowIfCancellationRequested();
                    await Task.WhenAll(currentPlayerPlacingTask, opositePlayerPlacingTask);
                    logger?.LogInformation("All Players setup pawns.");
                    TurnManager_TurnChanged(this, 0);
                }
            }

            using TurnManager turnMgr = new(map, playerManager, internationalWaterManager);

            while (!endDetector.GameIsEnded)
            {
                await playerManager.GetOpositePlayer()
                    .UserInterface.DisplayMessage("Poczekaj aż przeciwnik wykona ruch", MessageType.SideMessage);
                Move move = await turnMgr.MakeMove(token);
                await executor.Execute(move);
                turnManager.NextTurn();
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
