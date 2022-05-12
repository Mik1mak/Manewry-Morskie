using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Krazownik : Unit
    {
        public override uint Step => 2;
        protected override IEnumerable<Type> StrongerUnitsIfTheyAttackFirst => Array.Empty<Type>();
        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;

        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(Krazownik),
            typeof(OkretRakietowy),
            typeof(Pancernik),
            typeof(OkretPodwodny),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Krążownik";
    }
}
