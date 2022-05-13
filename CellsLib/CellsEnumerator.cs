using System.Collections;
using System.Collections.Generic;

namespace CellLib
{
    public class CellsEnumerator<T> : IEnumerator<T> where T : notnull
    {
        private readonly T[][] fields;

        private int currentRow = 0;
        private int currentColumn = -1;

        public CellsEnumerator(T[][] fields)
        {
            this.fields = fields;
        }

        public T Current => fields[currentColumn][currentRow];
        object IEnumerator.Current => fields[currentColumn][currentRow];

        public void Dispose(){}

        public bool MoveNext()
        {
            currentColumn++;

            if(currentColumn == fields.Length)
            {
                currentColumn = 0;
                currentRow++;
            }

            return currentRow != fields[0].Length;
        }

        public void Reset()
        {
            currentRow = 0;
            currentColumn = -1;
        }
    }
}
