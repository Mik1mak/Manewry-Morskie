using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class BoardTransformService
    {
        public int Scale { get; protected set; } = 100;
        public bool Horizontal { get; protected set; } = false;

        public event Func<int, bool, Task>? TransformationChanged;

        public async Task Zoom(int newScale)
        {
            Scale = newScale;

            if(TransformationChanged is not null)
                await TransformationChanged.Invoke(Scale, Horizontal);
        }

        public async Task Rotate(bool horizontal)
        {
            this.Horizontal = horizontal;

            if (TransformationChanged is not null)
                await TransformationChanged.Invoke(Scale, horizontal);
        }
    }
}
