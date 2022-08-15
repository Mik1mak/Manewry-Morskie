using CellLib;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ManewryMorskie.GameEndManagerComponents;
using ManewryMorskie.TurnManagerComponents;

namespace ManewryMorskie
{
    public class ManewryMorskieGame
    {
        private InternationalWaterManager internationalWaterManager;
        private StandardMap map;
        private GameEndManager? endManager;
        private PlayerManager playerManager;
        private MoveExecutor executor;
        private PawnHider pawnHider;

        private readonly ILogger? logger;
        private readonly TurnCounter turnManager = new();

        public bool AsyncGame { get; set; }

        public event EventHandler<int>? TurnChanged
        {
            add => turnManager.TurnChanged += value;
            remove => turnManager.TurnChanged -= value;
        }

        public ManewryMorskieGame(Player player1, Player player2, ILogger? logger = null)
        {
            this.logger = logger;
            playerManager = new PlayerManager(turnManager, player1, player2);

            map = StandardMap.DefaultMap(playerManager);

            internationalWaterManager = new InternationalWaterManager(map);

            turnManager.TurnChanging += TurnCounter_TurnChanging;
            internationalWaterManager.InternedUnit += InternationalWaterManager_InternedUnit;

            executor = new MoveExecutor(map, playerManager);
            pawnHider = new PawnHider(map, executor, playerManager, turnManager);
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
                    player.Fleet.Destroy(e);
                    field.Unit = null;

                    foreach (IUserInterface ui in playerManager.UniqueInferfaces)
                    {
                        await ui.TakeOffPawn(location);
                        await ui.DisplayMessage($"Jednostka {e} została internowana, ponieważ przebywała przez " +
                            "4 tury na wodach międzynarodowych!");
                    }
                    return;
                }
            }   
        }

        public async Task Start(CancellationToken token)
        {
            pawnHider.RegisterEvents(AsyncGame, turnManager);

            logger?.LogInformation("Game Started.");
            using (IPlacingManager currentPlacingMgr = new PawnsPlacingManager(map, playerManager, playerManager.CurrentPlayer, logger))
            {
                Task currentPlayerPlacingTask = currentPlacingMgr.PlacePawns(token);
                token.ThrowIfCancellationRequested();

                Player opositePlayer = playerManager.GetOpositePlayer();

                if (!AsyncGame)
                {
                    await Task.WhenAll(currentPlayerPlacingTask);
                    logger?.LogInformation("First Player setup pawns in async game.");
                    turnManager.NextTurn();
                }

                using (IPlacingManager opositePlacingMgr =
                    new PawnsPlacingManager(map, playerManager, opositePlayer, logger))
                {
                    Task opositePlayerPlacingTask = opositePlacingMgr.PlacePawns(token);
                    token.ThrowIfCancellationRequested();
                    await Task.WhenAll(currentPlayerPlacingTask, opositePlayerPlacingTask);
                    logger?.LogInformation("All Players setup pawns.");
                    turnManager.NextTurn();
                }
            }

            endManager = new GameEndManager(map, turnManager, playerManager, executor);

            using TurnManager turnMgr = new(map, playerManager, internationalWaterManager);

            while (!endManager.GameIsEnded)
            {
                await playerManager.GetOpositePlayer()
                    .UserInterface.DisplayMessage("Poczekaj aż przeciwnik wykona ruch", MessageType.SideMessage);
                Move move = await turnMgr.MakeMove(token);
                await executor.Execute(move);
                turnManager.NextTurn();
                token.ThrowIfCancellationRequested();
            }

            await playerManager.WriteToPlayers("Gra zakończona", MessageType.SideMessage);
        }
    }
}
