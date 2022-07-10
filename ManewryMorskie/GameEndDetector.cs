using CellLib;
using System;
using System.Linq;

namespace ManewryMorskie
{
    public enum GameEndReason
    {
        DestroyedBatteries,
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
        private readonly RectangleCellMap<MapField> map;
        private readonly TurnCounter turnCommander;
        private readonly PlayerManager playersManager;

        public event EventHandler<GameEnd>? GameEnded;


        public GameEndDetector(RectangleCellMap<MapField> map, TurnCounter turnCommander, PlayerManager playersManager)
        {
            this.map = map;
            this.turnCommander = turnCommander;
            this.playersManager = playersManager;

            this.turnCommander.TurnChanged += CheckGameEnds;
        }

        private void CheckGameEnds(object sender, int currentTurn)
        {
            Player current = playersManager.GetPlayerOfTurn(currentTurn);
            Player enemy = playersManager.GetOpositePlayer(current);

            if (enemy.Fleet.DestroyedUnitsCount<Bateria>() == 4)
            {
                GameEnded?.Invoke(this, new GameEnd(current, GameEndReason.DestroyedBatteries));
                return;
            }
            else if(enemy.Fleet.DestroyedUnitsCount<OkretDesantowy>() == 1)
            {
                GameEnded?.Invoke(this, new GameEnd(current, GameEndReason.DestroyedOkretDesantowy));
                return;
            }

            foreach (MapField field in map)
                if(field.Owner == enemy && field.Unit != null)
                    if(current.Fleet.Units.Contains(field.Unit))
                    {
                        GameEnded?.Invoke(this, new GameEnd(current, GameEndReason.OkretDesantowyReachedEnemyField));
                        return;
                    }

        }
    }
}
