using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManewryMorskie
{
    public class StandardMap : RectangleCellMap<MapField>
    {
        private const int STANDARD_WIDTH = 12;
        private const int STANDARD_HEIGHT = 18;

        public event EventHandler MapChanged;

        public static IEnumerable<CellLocation> DefaultTopEnterences { get; } = new CellLocation[]
        {
            (2, 16),
            (9, 14),
        };

        public static IEnumerable<CellLocation> DefaultBottomEnterences { get; } = new CellLocation[]
        {
            (2, 3),
            (9, 1),
        };

        private StandardMap(PlayerManager players) : base(STANDARD_WIDTH, STANDARD_HEIGHT)
        {
            BarrierBuilder barierBuilder = new(this);
            barierBuilder
                .AddRange((0, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((3, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((5, 1).NextLocations(Ways.Right, 3).Select(l => (l, l + Ways.Top)))
                .AddRange((10, 2).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((1, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Right)))
                .AddRange((4, 3).NextLocations(Ways.Bottom).Select(l => (l, l + Ways.Right)))
                .AddRange((10, 2).NextLocations(Ways.Bottom).Select(l => (l, l + Ways.Left)))
                .Add(((8, 1), (9, 1)))
                .AddSimmetricBarriers()
                .BuildBarriers();

            List<CellLocation> bottomTerritory = new();
            bottomTerritory.AddRange((0, 0).Region((4, 3)));
            bottomTerritory.AddRange((5, 0).Region((11, 1)));
            bottomTerritory.Add((10, 2));
            bottomTerritory.Add((11, 2));

            foreach (CellLocation location in bottomTerritory)
            {
                this[location].Owner = players.BottomPlayer;
                MiddleSimmetricElementTo(location).Owner = players.TopPlayer;
            }

            this.MarkInternationalWaters((5, 8).NextLocations(Ways.All))
                .MarkInternationalWaters((6, 9).NextLocations(Ways.All))
                .MarkInternationalWaters((6, 6), (5, 11));

            foreach (MapField mapField in this)
                mapField.PropertyChanged += MapField_PropertyChanged;
        }

        private void MapField_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) 
            => MapChanged?.Invoke(sender, EventArgs.Empty);

        public static StandardMap DefaultMap(PlayerManager players) => new(players);

        public override MapField this[CellLocation key] 
        { 
            get => base[key];
            protected set
            {
                if(base[key] != value)
                {
                    base[key] = value;
                    MapChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
            MapChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
