using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Eskortowiec : Unit
    {
        public override uint Step => 3;

        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;
        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(Pancernik),
            typeof(OkretRakietowy),
            typeof(Krazownik),
            typeof(Niszczyciel),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Eskortowiec";
    }
}
