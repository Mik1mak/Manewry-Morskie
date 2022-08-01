using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public class PlayerManager : IEnumerable<Player>
    {
        private readonly TurnCounter turnCommander;

        public PlayerManager(TurnCounter turnCommander, Player bottomPlayer, Player topPlayer)
        {
            this.turnCommander = turnCommander;
            BottomPlayer = bottomPlayer;
            TopPlayer = topPlayer;
        }

        public Player TopPlayer { get; private set; }
        public Player BottomPlayer { get; private set; }
        public Player CurrentPlayer => GetPlayerOfTurn(turnCommander.TurnNumber);
        public Player GetPlayerOfTurn(int turnNumber) => turnNumber % 2 == 0 ? BottomPlayer : TopPlayer;

        public IEnumerable<Player> GetOpositePlayers(Player current) => this.Where(p => current != p);
        public IEnumerable<Player> GetOpositePlayers() => this.Where(p => CurrentPlayer != p);

        public Player GetOpositePlayer(Player current) => this.First(p => current != p);
        public Player GetOpositePlayer() => this.First(p => CurrentPlayer != p);

        public void WriteToPlayers(Player current, string msgToCurrent, string msgToOthers)
        {
            current.UserInterface.DisplayMessage(msgToCurrent);

            foreach (Player other in GetOpositePlayers(current))
                other.UserInterface.DisplayMessage(msgToOthers);
        }

        public void WriteToPlayers(string msgToCurrent, string msgToOthers) => WriteToPlayers(CurrentPlayer, msgToCurrent, msgToOthers);


        public HashSet<IUserInterface> UniqueInferfaces => new(this.Select(p => p.UserInterface));

        public IEnumerator<Player> GetEnumerator()
        {
            yield return BottomPlayer;
            yield return TopPlayer;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
