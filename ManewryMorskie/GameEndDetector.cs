using CellLib;
using System;
using System.Linq;

namespace ManewryMorskie
{
    public enum GameEndReason
    {
        DestroyedOkretyRakietowe,
        DestroyedOkretDesantowy,
        OkretDesantowyReachedEnemyField,
    }

    public struct GameEnd
    {
        public Player? Winner { get; set; }
        public GameEndReason GameEndReason { get; set; }

        public GameEnd(Player? winner, GameEndReason gameEndReason)
        {
            Winner = winner;
            GameEndReason = gameEndReason;
        }
    }

    public class GameEndDetector
    {
        private readonly StandardMap map;
        private readonly TurnCounter turnCommander;
        private readonly PlayerManager playersManager;

        public event EventHandler<GameEnd>? GameEnded;
        public bool GameIsEnded { get; private set; } = false;
        public GameEnd? GameEndResult { get; private set; } = null;

        public GameEndDetector(StandardMap map, TurnCounter turnCommander, PlayerManager playersManager)
        {
            this.map = map;
            this.turnCommander = turnCommander;
            this.playersManager = playersManager;

            this.turnCommander.TurnChanged += CheckGameEnds;
            this.turnCommander.TurnChanging += CheckGameEnds;
        }

        private void CheckGameEnds(object sender, int currentTurn)
        {
            Player current = playersManager.GetPlayerOfTurn(currentTurn);
            Player enemy = playersManager.GetOpositePlayer(current);

            //jeśli wejścia gracza są chronione a przeciwnik stracił wszystkie okrętyRakietowe
            if (enemy.Fleet.ActiveUnitsCount<OkretRakietowy>() == 0)
            {
                if(playersManager.TopPlayer == current ? map.TopEntrencesAreProtected : map.BottomEntrecesAreProtected)
                {
                    EndGame(current, GameEndReason.DestroyedOkretyRakietowe);
                    return;
                }
                
            }
            else if(enemy.Fleet.ActiveUnitsCount<OkretDesantowy>() == 0) //jeśli przeciwnik stracił wszystkie okręty desantowe
            {
                EndGame(current, GameEndReason.DestroyedOkretDesantowy);
                return;
            }

            //jeśli okręt desantowy wpłynął do portu przeciwnika
            foreach (MapField field in map)
                if(field.Owner == enemy && field.Unit != null)
                    if(current.Fleet.Units.Contains(field.Unit) && field.Unit.GetType() == typeof(OkretDesantowy))
                    {
                        EndGame(current, GameEndReason.OkretDesantowyReachedEnemyField);
                        return;
                    }

        }

        private void EndGame(Player currentPlayer, GameEndReason reason)
        {
            GameEndResult = new GameEnd(currentPlayer, reason);
            GameIsEnded = true;
            GameEnded?.Invoke(this, GameEndResult.Value);
        }
    }
}
