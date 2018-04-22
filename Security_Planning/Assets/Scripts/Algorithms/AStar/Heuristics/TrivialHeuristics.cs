using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Algorithms.AStar.Heuristics
{
    /// <summary>
    /// Trivial heuristics returning always 0. Mainly for test purposes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TrivialHeuristics<T> : Heuristics<T> where T : IAStarNode<T>
    {
        public override PriorityCost ComputeHeuristics(T from, T to, int priorityCostLength)
        {
            return new PriorityCost(priorityCostLength, 0);
        }
    }
}
