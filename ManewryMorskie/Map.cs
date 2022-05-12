using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ManewryMorskie
{
    public abstract class Player
    {
        public string Name { get; set; } = nameof(Player);
    }

    public class MapField
    {
        public Ways Barriers { get; set; } = Ways.None;
        public Player? Owner { get; set; }
        public Unit? Unit { get; set; }
    }

    public class Board
    {
        private readonly InternationalWaterManager internationalWaterManager; //TODO
        protected Map map;

        public Board(Map map)
        {
            this.map = map;
        }

        public void Place(Unit unit)
        {
            internationalWaterManager.Track(unit);
        }

        public Unit Take(MapPoint location)
        {
            internationalWaterManager.Untrack(map[location].Unit);
            return map[location].Unit;
        }
    }

    public class Map : IEnumerable<MapField>
    {
        protected MapField[][] fields;

        public int Width => fields.Length; //columns
        public int Height => fields[0].Length; //rows

        public Map(MapField[][] fields)
        {
            this.fields = fields;
        }

        public Map(int height, int width)
        {
            fields = new MapField[height][];

            for(int row = 0; row < height; row++)
            {
                fields[row] = new MapField[width];

                for(int col = 0; col < width; col++)
                    fields[row][col] = new MapField();
            }
        }

        public MapField this[MapPoint key]
        {
            get => fields[Height - key.Row][key.Column];
            protected set => fields[Height - key.Row][key.Column] = value;
        }

        public IEnumerator<MapField> GetEnumerator() => new FieldEnumerator(fields);
        IEnumerator IEnumerable.GetEnumerator() => new FieldEnumerator(fields);
    }
}
