using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public struct Pawn
    {
        public bool LabelIsHidden { get; set; }

        public string? Label { get; set; }

        public int Color { get; init; }

        public bool IsBattery { get; init; }

        public Pawn Copy(string newLabel)
        {
            Pawn cpy = this;
            cpy.Label = newLabel;
            cpy.LabelIsHidden = string.IsNullOrEmpty(newLabel);
            return cpy;
        }
    }
}
