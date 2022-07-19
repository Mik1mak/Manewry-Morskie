using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Mina : Unit
    {
        public override uint Step => 0;
        protected override IEnumerable<Type> StrongerUnits => Array.Empty<Type>();
        public override string ToString() => "Mina";
    }
}
