using CellLib;
using System.Collections.Generic;
using System;

namespace ManewryMorskie
{
    public class Move
    {
        public CellLocation From { get; set; }
        public CellLocation To { get; set; }

        private CellLocation? disarm;
        public CellLocation? Disarm
        {
            get => disarm;
            set
            {
                if (!attack.HasValue && ((value.HasValue && To.IsInRadius(value.Value, 1)) || !value.HasValue))
                    disarm = value;
                else
                    throw new InvalidOperationException();
            }
        }

        private CellLocation? attack;
        public CellLocation? Attack 
        {
            get => attack;
            set
            {
                if (!disarm.HasValue && ((value.HasValue && To.IsInRadius(value.Value, 1)) || !value.HasValue))
                    attack = value;
                else
                    throw new InvalidOperationException();
            }
        }
        public IEnumerable<CellLocation> Path { get; set; } = new HashSet<CellLocation>();
    }
}