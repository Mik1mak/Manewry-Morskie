using System;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public abstract class Unit
    {
        protected abstract IEnumerable<Type> StrongerUnits { get; }
        protected abstract IEnumerable<Type> StrongerUnitsIfTheyAttackFirst { get; }

        public abstract uint Step { get; }
        public virtual uint AttackRange => Step;

        public (bool canBeDestroyed, bool onlyIfAttackFirst) CanBeDestroyedBy(Unit u)
        {
            bool simplyStronger = StrongerUnits.Any(t => t == u.GetType());
            bool conditionalStronger = StrongerUnitsIfTheyAttackFirst.Any(t => t == u.GetType());
            
            return (simplyStronger || conditionalStronger, conditionalStronger);
        }
            
    }
}
