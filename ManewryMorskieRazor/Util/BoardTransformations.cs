using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor.Util
{
    public record BoardTransformations
    {
        public const int DEFAUKT_ZOOM = 100;

        public int Zoom { get; init; } = DEFAUKT_ZOOM;
        public bool IsHorizontal { get; init; }
        public double TranslateXpixels { get; init; }
        public double TranslateYpixels { get; init; }
    }
}
