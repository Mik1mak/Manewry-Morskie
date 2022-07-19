using CellLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public class MoveExecutor
    {
        private readonly StandardMap map;
        private readonly PlayerManager players;

        public MoveExecutor(StandardMap map, PlayerManager players)
        {
            this.map = map;
            this.players = players;
        }

        public async Task Execute(Move move)
        {
            move.Result = GetResult(move);

            if ((move.Result & BattleResult.TargetDestroyed) != BattleResult.None)
                DestroyUnit((move.Attack ?? move.Disarm)!.Value, players.GetOpositePlayer());

            if ((move.Result & BattleResult.SourceDestroyed) != BattleResult.None)
            {
                DestroyUnit(move.From, players.CurrentPlayer);
            }
            else
            {
                map[move.To].Unit = map[move.From].Unit;
                map[move.From].Unit = null;
            }

            foreach (Player player in players)
                await player.UserInterface.ExecuteMove(move);
        }

        private BattleResult GetResult(Move move)
        {
            if (move.Attack.HasValue)
            {
                return map[move.Attack.Value].Unit!.AttackedBy(map[move.From].Unit!);
            }
            else if(move.Disarm.HasValue)
            {
                if (map[move.Disarm.Value].Unit is Mina)
                    return BattleResult.TargetDestroyed;
                else
                    return BattleResult.SourceDestroyed;
            }
            else
            {
                return BattleResult.None;
            }
        }

        private void DestroyUnit(CellLocation unitLocation, Player unitOwner)
        {
            Unit destroyedUnit = map[unitLocation].Unit!;
            map[unitLocation].Unit = null;
            unitOwner.Fleet.Destroy(destroyedUnit);
        }
    }
}
