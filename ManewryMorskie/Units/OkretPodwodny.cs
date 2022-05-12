using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class OkretPodwodny : Unit
    {
        public override uint Step => 2;

        protected override IEnumerable<Type> StrongerUnitsIfTheyAttackFirst => _strongerUnitsIfTheyAttackFirst;
        private readonly static Type[] _strongerUnitsIfTheyAttackFirst = new Type[]
        {
            typeof(Niszczyciel),
        };
        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;
        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(OkretPodwodny),
            typeof(Eskortowiec),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Okręt podwodny";
    }
}
