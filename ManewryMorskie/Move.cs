using CellLib;
using System.Collections.Generic;
using System;

namespace ManewryMorskie
{
    public enum BattleResult
    {
        None,
        TargetDestroyed,
        SourceDestroyed,
        Draw,
    }

    public class Move
    {
        public CellLocation From { get; set; }
        public CellLocation To { get; set; }
        public string SourceUnitDescription { get; set; } = string.Empty;
        public string? TargetUnitDescription { get; set; }

        public CellLocation? Disarm { get; set; }
        public CellLocation? Attack { get; set; }

        public BattleResult Result { get; set; } = BattleResult.None;

        public HashSet<CellLocation> SetMines { get; set; } = new();
        public IEnumerable<CellLocation> Path { get; set; } = Array.Empty<CellLocation>();

        public void Clear()
        {
            SourceUnitDescription = string.Empty;
            TargetUnitDescription = null;
            Disarm = Attack = null;
            Result = BattleResult.None;
            Path = Array.Empty<CellLocation>();
            SetMines.Clear();
        }
    }
}