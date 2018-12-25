using System.Collections.Generic;
using System.Linq;
using System;
using Athanor.Collections.Generic;

namespace GridLib.Pathing
{
    public class PathNode<TCoords> : IComparable<PathNode<TCoords>> where TCoords : IEquatable<TCoords>
    {
        public int costToNode;
        public int estToGoal;
        public PathNode<TCoords> previousNode;
        public TCoords loc;

        public PathNode(int costToNode, PathNode<TCoords> previousNode, TCoords loc)
        {
            this.costToNode = costToNode;
            this.previousNode = previousNode;
            this.loc = loc;
        }

        public PathNode(int costToNode, int estToGoal, PathNode<TCoords> previousNode, TCoords loc)
        {
            this.costToNode = costToNode;
            this.estToGoal = estToGoal;
            this.previousNode = previousNode;
            this.loc = loc;
        }

        public IEnumerable<TCoords> pathBack
        {
            get
            {
                for (PathNode<TCoords> cursor = this; cursor != null; cursor = cursor.previousNode)
                    yield return cursor.loc;
            }
        }

        public IEnumerable<TCoords> pathTo { get { return pathBack.Reverse(); } }

        public int heuristic { get { return (estToGoal < int.MaxValue - costToNode) ? (costToNode + estToGoal) : (int.MaxValue); } }

        public int CompareTo(PathNode<TCoords> other)
        {
            return other.heuristic - heuristic;
        }

        public override string ToString()
        {
            return "PathNode: " + loc + " (c" + costToNode + ", e" + estToGoal + ")";
        }
    }

    public interface ICanPath<TCoords> where TCoords : IEquatable<TCoords>
    {
        TCoords loc { get; }
        IEnumerable<TCoords> ValidNeighbors(TCoords loc);

        bool CanEnter(TCoords loc);
        bool CanStay(TCoords loc);
        bool CanLeave(TCoords loc);

        int CostToEnter(TCoords loc);
        int Heuristic(TCoords src, TCoords dst);
    }

    internal static class PathingExt
    {
        #region Algorithms

        private static bool IsRedundant<TCoords>(
            this Dictionary<TCoords, PathNode<TCoords>> result,
            PathNode<TCoords> candidate)
            where TCoords : IEquatable<TCoords>
        {
            if (!result.Keys.Contains(candidate.loc)) return false;
            else return result[candidate.loc].costToNode <= candidate.costToNode;
        }

        public static IDictionary<TCoords, PathNode<TCoords>> Dijkstra<TCoords>(
            this ICanPath<TCoords> unit,
            int maxCost = int.MaxValue)
            where TCoords : IEquatable<TCoords>
        {
            Dictionary<TCoords, PathNode<TCoords>> result = new Dictionary<TCoords, PathNode<TCoords>>();

            Queue<PathNode<TCoords>> frontier = new Queue<PathNode<TCoords>>();
            frontier.Enqueue(new PathNode<TCoords>(
                0,
                null,
                unit.loc));

            while (frontier.Count > 0)
            {
                PathNode<TCoords> current = frontier.Dequeue();

                // If we already have a path to this location that's at least this cheap,
                // don't repeat it
                if (!result.IsRedundant(current))
                {
                    // If we can stay in the current location, mark it down as a destination
                    result[current.loc] = current;

                    // If we can leave the current location, try entering neighboring locations
                    int movesRemaining = maxCost - current.costToNode;
                    if ((movesRemaining > 0) && unit.CanLeave(current.loc))
                    {
                        // If we can enter any neighboring locations, try to do so later.
                        IEnumerable<TCoords> neighbors = unit
                            .ValidNeighbors(current.loc)
                            .Where(unit.CanEnter);
                        foreach(TCoords neighbor in neighbors)
                            frontier.Enqueue(new PathNode<TCoords>(
                                current.costToNode + 1,
                                current,
                                neighbor));
                    }
                }
            }

            // No need to include where we're standing now
            result.Remove(unit.loc);

            return result;
        }

        public static IEnumerable<TCoords> AStar<TCoords>(
            this ICanPath<TCoords> unit,
            TCoords dst,
            int maxCost = int.MaxValue)
            where TCoords : IEquatable<TCoords>
        {
            Dictionary<TCoords, PathNode<TCoords>> result = new Dictionary<TCoords, PathNode<TCoords>>();

            PriorityQueue<PathNode<TCoords>> frontier = new PriorityQueue<PathNode<TCoords>>();
            frontier.Enqueue(new PathNode<TCoords>(
                0,
                unit.Heuristic(unit.loc, dst),
                null,
                unit.loc));

            while (frontier.Count > 0)
            {
                PathNode<TCoords> current = frontier.Dequeue();

                // If we've arrived
                if (current.loc.Equals(dst)) return current.pathTo.Skip(1);

                // If we already have a path to this location that's at least this cheap,
                // don't repeat it
                if (!result.IsRedundant(current))
                {
                    // If we can stay in the current location, mark it down as a destination
                    result[current.loc] = current;

                    // If we can leave the current location, try entering neighboring locations
                    int movesRemaining = maxCost - current.costToNode;
                    if ((movesRemaining > 0) && unit.CanLeave(current.loc))
                    {
                        // If we can enter any neighboring locations, try to do so later.
                        IEnumerable<TCoords> neighbors = unit
                            .ValidNeighbors(current.loc)
                            .Where(unit.CanEnter);
                        foreach (TCoords neighbor in neighbors)
                            frontier.Enqueue(new PathNode<TCoords>(
                                current.costToNode + 1,
                                current,
                                neighbor));
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
