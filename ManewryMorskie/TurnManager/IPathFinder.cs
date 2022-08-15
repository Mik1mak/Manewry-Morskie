using CellLib;
using System.Collections.Generic;
using System.Text;

namespace ManewryMorskie.TurnManagerComponents
{
    public interface IPathFinder
    {
        public IEnumerable<CellLocation> CellsWhereDistanceFromSourceIsLowerThan(uint distance);
        public IEnumerable<CellLocation> ShortestPathTo(CellLocation target);
    }
}
