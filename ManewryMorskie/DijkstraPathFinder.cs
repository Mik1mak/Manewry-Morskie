using CellLib;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public class DijkstraPathFinder : IPathFinder
    {
        private StandardMap map;
        private CellLocation pivot;

        private readonly Dictionary<CellLocation, int> distances = new();
        private readonly Dictionary<CellLocation, CellLocation?> previousCells = new();

        public DijkstraPathFinder(StandardMap map, CellLocation source)
        {
            this.map = map;
            this.pivot = source;

            Djikstra();
        }

        private void Djikstra()
        {
            distances[pivot] = 0;
            SimplePriorityQueue<CellLocation> queue = new();

            foreach (CellLocation location in map.Keys)
            {
                if(location != pivot)
                {
                    distances[location] = int.MaxValue;
                    previousCells[location] = null;
                }
                queue.Enqueue(location, distances[location]);
            }

            while(queue.Count > 0)
            {
                CellLocation tmp = queue.Dequeue();
                Ways neighborhood = map.AvaibleWaysFrom(tmp);

                foreach (CellLocation neighbor in neighborhood.EverySingleWay().Select(way => tmp + way).Where(l => queue.Contains(l)))
                {
                    int alt = distances[tmp] + 1;

                    if(alt < distances[neighbor])
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
            return distances.Where(kvp => kvp.Value > 0 && kvp.Value < distance).Select(kvp => kvp.Key);
        }

        public IEnumerable<CellLocation> ShortestPathTo(CellLocation target)
        {
            List<CellLocation> output = new();

            while (previousCells[target] != pivot)
            {
                output.Add(previousCells[target]!.Value);
                target = previousCells[target]!.Value;
            }

            return output;
        }
    }
}
