using CellLib;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{

    public class ManewryMorskieGame : IGame
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
            if(new Random().Next(0, 2) == 0)
                playerManager = new(turnManager, player1, player2);
            else
                playerManager = new(turnManager, player2, player1);

            map = StandardMap.DefaultMap(playerManager);

            internationalWaterManager = new InternationalWaterManager(map);

            turnManager.TurnChanging += TurnCounter_TurnChanging;
            internationalWaterManager.InternedUnit += InternationalWaterManager_InternedUnit;

            endDetector = new GameEndDetector(map, turnManager, playerManager);
            endDetector.GameEnded += EndDetector_GameEnded;

            executor = new(map, playerManager);
        }

        private void EndDetector_GameEnded(object sender, GameEnd e)
        {
            throw new NotImplementedException();

            switch(e.GameEndReason)
            {
                case GameEndReason.DestroyedOkretDesantowy:
                    playerManager.WriteToPlayers(e.Winner!,
                        msgToCurrent: "Zwycięstwo! Zniszczyłeś okręt desantowy przeciwnika!",
                        msgToOthers: "Porażka! Przeciwnik zniszczył Twój okręt desantowy!");
                    break;
                case GameEndReason.DestroyedOkretyRakietowe:

                    break;
                case GameEndReason.OkretDesantowyReachedEnemyField:

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
                    foreach (Player player in playerManager)
                    {
                        await player.UserInterface.TakeOffPawn(location);

                        if (player.Fleet.Units.Contains(e))
                        {
                            await player.UserInterface.DisplayMessage($"Jednostka {e} została internowana, ponieważ przebywała przez " +
                                $"{internationalWaterManager.TurnsOnInternationalWaterLimit} tury na wodach międzynarodowych!");
                            player.Fleet.Destroy(e);
                        }
                    }
                        

                    field.Unit = null;
                    return;
                }
            }   
        }

        public async Task Start(CancellationToken token)
        {
            //ustawianie: predefiniowane, własne - przez listę - format: >jednostaka (dostępnych)
            IPlacingManager currentPlacingMgr = new PawnsPlacingManager(map, playerManager, playerManager.CurrentPlayer);
            Task currentPlayerPlacingTask = currentPlacingMgr.PlacePawns(token);

            if (!AsyncGame)
                Task.WaitAll(currentPlayerPlacingTask);

            IPlacingManager opositePlacingMgr
                = new PawnsPlacingManager(map, playerManager, playerManager.GetOpositePlayer(playerManager.CurrentPlayer));
            Task opositePlayerPlacingTask = opositePlacingMgr.PlacePawns(token);

            Task.WaitAll(currentPlayerPlacingTask, opositePlayerPlacingTask);

            //rozgrywka:
            //kolorowanie pól pionków zdolnych do ruchu | ustawienia miny jeśli trałowiec
            //zaznaczenie pionka: wybranie go
            //kolorowanie dostępnych pól do poruszenia się lub postawienia miny i statków do zaatakowania lub rozbrojenia
            //wybranie pola do przemieszczenia się lub postawienia miny - wybór między przemieszczeniem a postawieniem jeśli trałowiec
            //wybranie przemieszczenia - wykonanie ruchu - koniec tury
            //wybranie postawienie - postawienie - pytanie o pozyche końcową
            //wybranie pozycji końcowej - wykonanie ruchu - koniec tury
            //wybranie pola do ataku lub rozbrojenia - wybór między atakiem a rozbrojeniem jeśli trałowiec
            //podświetlenie możliwych pozycji końcowych
            //wybranie pozycji końcowej - wykonanie ruchu, koniec tury

            TurnManager turnMgr = new(map, playerManager);

            while (!endDetector.GameIsEnded)
            {
                Move move = await turnMgr.MakeMove(token);
                await executor.Execute(move);

                turnManager.NextTurn();
            }

        }

        public Task PauseOrResume()
        {
            throw new NotImplementedException();
        }

        public Task Reset()
        {
            throw new NotImplementedException();
        }
    }
}
