using CellLib;

namespace ManewryMorskie
{
    public class MapField
    {
        public bool InternationalWater { get; set; } = false;
        public Ways Barriers { get; set; } = Ways.None;
        public Player? Owner { get; set; }
        public Unit? Unit { get; set; }
    }
}
