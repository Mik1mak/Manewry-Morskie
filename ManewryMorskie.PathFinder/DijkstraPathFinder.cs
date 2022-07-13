using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskie.PathFinder;

public class DijkstraPathFinder : IPathFinder
{
    private StandardMap map;
    private CellLocation pivot;

    private readonly Dictionary<CellLocation, int> distances = new();
    private readonly Dictionary<CellLocation, int> previousCells = new();

    public DijkstraPathFinder(StandardMap map, CellLocation source)
    {
        this.map = map;
        this.pivot = source;

        Djikstra();
    }

    private void Djikstra()
    {
        distances[pivot] = 0;

        PriorityQueue<CellLocation, int> queue = new();

        //for (i = 0; i < numOfVertex; i++)
        //{
        //    if (i != startVertex)
        //    {
        //        distance[i] = INT_MAX;
        //        previous[i] = INT_MAX;
        //    }
        //    addElement(&queue, i, distance[i]);
        //}
    }

    public IEnumerable<CellLocation> CellsWhereDistanceFromSourceIsLowerThan(uint distance)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CellLocation> ShortestPathTo(CellLocation target)
    {
        throw new NotImplementedException();
    }
}

