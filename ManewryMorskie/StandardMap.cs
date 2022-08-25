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

        public event EventHandler? MapChanged;

        private readonly Dictionary<Unit, CellLocation> unitsLocations = new();
        public IReadOnlyDictionary<Unit, CellLocation> UnitsLocations => unitsLocations;

        public bool TopEntrencesAreProtected => EntrencesIsProtected(DefaultTopEnterences);
        public bool BottomEntrecesAreProtected => EntrencesIsProtected(DefaultBottomEnterences);

        private bool EntrencesIsProtected(IEnumerable<CellLocation> entrecesLocations)
        {
            return entrecesLocations
                .All(entrenceLocation => CellLib.Extensions.AllDirections
                    .Any(way => this[entrenceLocation + way].Unit is Bateria));
        }

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
                .AddRange((5, 8).NextLocations(Ways.Right).Select(l => (l, l + Ways.Bottom)))
                .Add(((6, 8), (7, 8)))
                .Add(((4, 8), (5, 8)))
                .Add(((8, 1), (9, 1)))
                .AddSymmetricBarriers()
                .BuildBarriers();

            List<CellLocation> bottomTerritory = new();
            bottomTerritory.AddRange((0, 0).Region((4, 3)));
            bottomTerritory.AddRange((5, 0).Region((11, 1)));
            bottomTerritory.Add((10, 2));
            bottomTerritory.Add((11, 2));

            foreach (CellLocation location in bottomTerritory)
            {
                this[location].Owner = players.BottomPlayer;
                this[CenterSymmetricKey(location)].Owner = players.TopPlayer;
            }

            this.MarkInternationalWaters((5, 8).NextLocations(Ways.All));
            this.MarkInternationalWaters((6, 9).NextLocations(Ways.All));
            this.MarkInternationalWaters((6, 6), (5, 11));

            foreach (CellLocation key in Keys)
            {
                this[key].Location = key;
                this[key].UnitsLocations = unitsLocations;
            }
        }

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

        public Ways AvaibleWaysFrom(CellLocation location)
        {
            Ways waysWithoutBarriers = ~this[location].Barriers;
            Ways output = Ways.None;

            foreach (Ways way in waysWithoutBarriers.EverySingleWay())
                if (this[location + way].Unit == null)
                    output |= way;

            return output;
        }
        
        public Ways AvaibleWaysFrom(CellLocation location, IEnumerable<CellLocation> forbidden)
        {
            Ways output = Ways.None;

            foreach (Ways way in AvaibleWaysFrom(location).EverySingleWay())
                if (!forbidden.Contains(location + way))
                    output |= way;

            return output;
        }

        public HashSet<CellLocation> CellsProtectedByBatteries(Player batteriesOwner)
        {
            return new HashSet<CellLocation>(
                    batteriesOwner.Fleet.Units
                        .Where(u => u is Bateria)
                        .Select(b => UnitsLocations[b])
                        .SelectMany(l => l.SquereRegion(1)));
        }

        public IEnumerable<CellLocation> LocationsWithPlayersUnits(Player player)
        {
            return player.Fleet.Units.Select(u => UnitsLocations[u]);
        }
    }
}
