using CellLib;
using System;
using System.Collections.Generic;
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

        private readonly TurnCounter turnManager = new();

        public bool AsyncPlacing { get; set; } = false;

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
                case GameEndReason.DestroyedBatteries:

                    break;
                case GameEndReason.OkretDesantowyReachedEnemyField:

                    break;
            }
        }

        private void TurnCounter_TurnChanging(object sender, int e)
        {
            internationalWaterManager.Iterate();
        }

        private void InternationalWaterManager_InternedUnit(object sender, Unit e)
        {
            foreach (MapField field in internationalWaterManager.InternationalWaters.Select(location => map[location]))
                if (field.Unit == e)
                {
                    field.Unit = null;
                    break;
                }

            foreach (Player player in playerManager)
                if(player.Fleet.Units.Contains(e))
                {
                    player.UserInterface.DisplayMessage($"Jednostka {e} została internowana, ponieważ przebywała przez " +
                        $"{internationalWaterManager.TurnsOnInternationalWaterLimit} tury na wodach międzynarodowych!");
                    player.Fleet.Destroy(e);
                    return;
                }
        }

        public Task Start(CancellationToken token)
        {
            //ustawianie: predefiniowane, własne - przez listę - format: >jednostaka (dostępnych)
            PlacePawns(token);

            throw new NotImplementedException();


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


            return Task.CompletedTask;
        }

        private void PlacePawns(CancellationToken token)
        {
            IPlacingManager currentPlacingMgr = new PawnsPlacingManager(map, playerManager, playerManager.CurrentPlayer);
            Task currentPlayerPlacingTask = Task.Run(async () => await currentPlacingMgr.PlacePawns(token));

            if (!AsyncPlacing)
                Task.WaitAll(currentPlayerPlacingTask);

            IPlacingManager opositePlacingMgr
                = new PawnsPlacingManager(map, playerManager, playerManager.GetOpositePlayer(playerManager.CurrentPlayer));
            Task opositePlayerPlacingTask = Task.Run(async () => await opositePlacingMgr.PlacePawns(token));

            Task.WaitAll(currentPlayerPlacingTask, opositePlayerPlacingTask);
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
