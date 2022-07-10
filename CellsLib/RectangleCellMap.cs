using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CellLib
{
    public class RectangleCellMap<T> : IEnumerable<T> where T : class, new()
    {
        protected T[][] fields;

        public int Width => fields.Length; //columns
        public int Height => fields[0].Length; //rows

        public IEnumerable<CellLocation> Keys => (0, 0).Region((Width-1, Height-1));

        public RectangleCellMap(T[][] fields)
        {
            this.fields = fields;
        }

        public RectangleCellMap(int width, int height)
        {
            fields = new T[width][];

            for(int col = 0; col < width; col++)
            {
                fields[col] = new T[height];

                for(int row = 0; row < height; row++)
                    fields[col][row] = new T();
            }
        }

        public CellLocation? LocationOf(T value)
        {
            foreach (CellLocation location in Keys)
                if (this[location] == value)
                    return location;

            return null;
        }

        public virtual T this[CellLocation key]
        {
            get => fields[key.Column][Height - 1 - key.Row];
            protected set => fields[key.Column][Height - 1 - key.Row] = value;
        }

        public T MiddleSimmetricElementTo(CellLocation key)
        {
            return this[new CellLocation(
                column: Width - 1 - key.Column,
                row: Height - 1 - key.Row)];
        }

        public virtual void Clear()
        {
            foreach (T[] lineofCells in fields)
                for (int i = 0; i < lineofCells.Length; i++)
                    lineofCells[i] = new();
        }

        public IEnumerator<T> GetEnumerator() => new CellsEnumerator<T>(fields);
        IEnumerator IEnumerable.GetEnumerator() => new CellsEnumerator<T>(fields);
    }
}
