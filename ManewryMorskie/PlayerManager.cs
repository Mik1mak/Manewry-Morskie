using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public class PlayerManager : IEnumerable<Player>
    {
        private readonly List<Player> players = new();
        private readonly TurnCounter turnCommander;

        public PlayerManager(TurnCounter turnCommander)
        {
            this.turnCommander = turnCommander;
        }

        public PlayerManager AddPlayer(Player player) 
        {
            players.Add(player);
            return this;
        }

        public Player CurrentPlayer => GetPlayerOfTurn(turnCommander.TurnNumber);
        public Player GetPlayerOfTurn(int turnNumber) => players[turnNumber % players.Count];

        public IEnumerable<Player> GetOpositePlayers(Player current) => players.Where(p => current != p);
        public IEnumerable<Player> GetOpositePlayers() => players.Where(p => CurrentPlayer != p);

        public Player GetOpositePlayer(Player current) => players.First(p => current != p);
        public Player GetOpositePlayer() => players.First(p => CurrentPlayer != p);

        public void WriteToPlayers(Player current, string msgToCurrent, string msgToOthers)
        {
            current.UserInterface.DisplayMessage(msgToCurrent);

            foreach (Player other in GetOpositePlayers(current))
                other.UserInterface.DisplayMessage(msgToOthers);
        }

        public void WriteToPlayers(string msgToCurrent, string msgToOthers) => WriteToPlayers(CurrentPlayer, msgToCurrent, msgToOthers);

        public IEnumerator<Player> GetEnumerator() => players.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => players.GetEnumerator();
    }
}
