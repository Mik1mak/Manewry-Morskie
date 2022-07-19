using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class Niszczyciel : Unit
    {
        public override uint Step => 4;

        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;
        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(OkretPodwodny),
            typeof(OkretRakietowy),
            typeof(Krazownik),
            typeof(Pancernik),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Niszczyciel";
    }
}
