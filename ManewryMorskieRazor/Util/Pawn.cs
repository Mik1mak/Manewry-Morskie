using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public struct Pawn
    {
        public string? Label { get; set; }

        public int Color { get; init; }

        public bool IsBattery { get; init; }
    }
}
