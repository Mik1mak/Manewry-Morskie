using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Niszczyciel : Unit
    {
        public override uint Step => 4;
        protected override IEnumerable<Type> StrongerUnitsIfTheyAttackFirst => _strongerUnitsIfTheyAttackFirst;
        private readonly static Type[] _strongerUnitsIfTheyAttackFirst = new Type[]
        {
            typeof(OkretPodwodny),
        };
        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;
        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(Niszczyciel),
            typeof(OkretRakietowy),
            typeof(Krazownik),
            typeof(Pancernik),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Niszczyciel";
    }
}
