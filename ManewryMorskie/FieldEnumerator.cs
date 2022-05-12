using System.Collections;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class FieldEnumerator : IEnumerator<MapField>
    {
        private MapField[][] fields;

        private int currentRow = 0;
        private int currentColumn = -1;

        public FieldEnumerator(MapField[][] fields)
        {
            this.fields = fields;
        }

        public MapField Current => fields[currentRow][currentColumn];
        object IEnumerator.Current => fields[currentRow][currentColumn];

        public void Dispose()
        {
            fields = null;
        }

        public bool MoveNext()
        {
            currentColumn++;

            if(currentColumn == fields.Length)
            {
                currentColumn = 0;
                currentRow++;
            }

            return currentRow != fields.Length;
        }

        public void Reset()
        {
            currentRow = 0;
            currentColumn = -1;
        }
    }
}
