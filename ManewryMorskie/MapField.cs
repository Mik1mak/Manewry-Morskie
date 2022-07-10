using CellLib;
using System.ComponentModel;

namespace ManewryMorskie
{
    public class MapField : INotifyPropertyChanged
    {
        public bool InternationalWater { get; set; } = false;
        public Ways Barriers { get; set; } = Ways.None;
        public Player? Owner { get; set; }

        private Unit? unit;
        public Unit? Unit 
        {
            get => unit;
            set
            {
                if(unit != value)
                {
                    unit = value;
                    PropertyChanged?.Invoke(this, new(nameof(Unit)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
