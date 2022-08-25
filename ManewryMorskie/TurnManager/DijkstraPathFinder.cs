using CellLib;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie.TurnManagerComponents
{
    public class DijkstraPathFinder : IPathFinder
    {
        private const int BASE_NODE_WEIGHT = 256;

        private StandardMap map;
        private CellLocation pivot;

        private readonly IEnumerable<CellLocation> forbidden;

        private readonly Dictionary<CellLocation, int> distances = new();
        private readonly Dictionary<CellLocation, CellLocation?> previousCells = new();

        public DijkstraPathFinder(StandardMap map, CellLocation source, IEnumerable<CellLocation> forbidden)
        {
            this.map = map;
            this.forbidden = forbidden;
            pivot = source;
            
            Djikstra();
        }

        private void Djikstra()
        {
            distances[pivot] = 0;
            SimplePriorityQueue<CellLocation> queue = new();

            foreach (CellLocation location in pivot.SquereRegion((int)map[pivot].Unit!.Step)
                .Where(l => map.LocationIsOnTheMap(l)))
            {
                if (location != pivot)
                {
                    distances[location] = int.MaxValue;
                    previousCells[location] = null;
                }
                queue.Enqueue(location, distances[location]);
            }

            while (queue.Count > 0)
            {
                CellLocation tmp = queue.Dequeue();
                Ways neighborhood = map.AvaibleWaysFrom(tmp, forbidden);

                foreach (var (neighbor, way) in neighborhood.EverySingleWay()
                    .Select(way => (tmp + way, way)).Where(l => queue.Contains(l.Item1)))
                {
                    //int alt = distances[tmp] + BASE_NODE_WEIGHT;
                    int alt = distances[tmp]
                        + (CellLib.Extensions.MainDirections.Contains(way) ? BASE_NODE_WEIGHT : BASE_NODE_WEIGHT + 1);

                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previousCells[neighbor] = tmp;

                        queue.UpdatePriority(neighbor, alt);
                    }
                }
            }
        }

        public IEnumerable<CellLocation> CellsWhereDistanceFromSourceIsLowerThan(uint distance)
        {
            return distances.Where(kvp => kvp.Value > 0 && kvp.Value < distance * BASE_NODE_WEIGHT).Select(kvp => kvp.Key);
        }

        public IEnumerable<CellLocation> ShortestPathTo(CellLocation target)
        {
            List<CellLocation> output = new();

            while (previousCells[target] != pivot)
            {
                output.Add(previousCells[target]!.Value);
                target = previousCells[target]!.Value;
            }

            output.Reverse();
            return output;
        }
    }
}
