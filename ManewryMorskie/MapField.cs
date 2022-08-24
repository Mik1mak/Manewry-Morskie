using CellLib;
using System.Collections.Generic;
using System.ComponentModel;

namespace ManewryMorskie
{


    public class MapField
    {
        internal Dictionary<Unit, CellLocation>? UnitsLocations { private get; set; }
        public CellLocation Location { get; internal set; }

        public bool InternationalWater { get; set; } = false;
        public Ways Barriers { get; set; } = Ways.None;
        public Player? Owner { get; set; }
        

        private Unit? unit;
        public Unit? Unit 
        { 
            get => unit;
            set
            {
                if(UnitsLocations != null && value != null)
                    UnitsLocations[value] = Location;

                unit = value;
            }
        }
    }
}
