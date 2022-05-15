using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Bateria : Unit
    {
        public override uint Step => 0;
        public override uint AttackRange => 1;

        protected override IEnumerable<Type> StrongerUnitsIfTheyAttackFirst => Array.Empty<Type>();
        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;

        private readonly static Type[] _strongerUnits = new Type[]
        {
            typeof(OkretRakietowy),
        };

        public override string ToString() => "Bateria nabrzeżna";
    }
}
