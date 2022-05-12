using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Tralowiec : Unit
    {
        public override uint Step => 2;

        protected override IEnumerable<Type> StrongerUnitsIfTheyAttackFirst => Array.Empty<Type>();
        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;
        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(Tralowiec),
            typeof(Eskortowiec),
            typeof(Pancernik),
            typeof(OkretRakietowy),
            typeof(Krazownik),
            typeof(Niszczyciel),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Trałowiec";
    }
}
