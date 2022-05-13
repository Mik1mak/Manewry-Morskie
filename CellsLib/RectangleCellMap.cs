using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CellLib
{
    public class RectangleCellMap<T> : IEnumerable<T> where T : new()
    {
        protected T[][] fields;

        public int Width => fields.Length; //columns
        public int Height => fields[0].Length; //rows

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

        public T this[CellLocation key]
        {
            get => fields[key.Column][Height - 1 - key.Row];
            protected set => fields[key.Column][Height - 1 - key.Row] = value;
        }

        public IEnumerator<T> GetEnumerator() => new CellsEnumerator<T>(fields);
        IEnumerator IEnumerable.GetEnumerator() => new CellsEnumerator<T>(fields);
    }
}
