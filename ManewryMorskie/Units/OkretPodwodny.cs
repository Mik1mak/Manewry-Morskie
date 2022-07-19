using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class OkretPodwodny : Unit
    {
        public override uint Step => 2;

        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;
        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(Niszczyciel),
            typeof(Eskortowiec),
            typeof(Mina),
            typeof(Bateria),
        };

        public override string ToString() => "Okręt podwodny";
    }
}
