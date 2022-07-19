using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class OkretRakietowy : Unit
    {
        public override uint Step => 1;
        protected override IEnumerable<Type> StrongerUnits => _strongerUnits;

        private readonly static Type[] _strongerUnits = new[]
        {
            typeof(OkretPodwodny),
            typeof(Pancernik),
            typeof(Mina),
        };

        public override string ToString() => "Okręt rakietowy";
    }
}
