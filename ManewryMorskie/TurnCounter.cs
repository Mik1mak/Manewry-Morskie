using System.Collections.Generic;
using System;

namespace ManewryMorskie
{

    public class TurnCounter
    {
        public int TurnNumber { get; private set; } = 0;

        public event EventHandler<int>? TurnChanging;
        public event EventHandler<int>? TurnChanged;

        public void NextTurn()
        {
            TurnChanging?.Invoke(this, TurnNumber);
            TurnNumber++;
            TurnChanged?.Invoke(this, TurnNumber);
        }

        internal void Reset() => TurnNumber = 0;
    }
}
