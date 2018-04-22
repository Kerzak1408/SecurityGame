using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Algorithms.AStar.Heuristics
{
    public abstract class Heuristics<T> where T : IAStarNode<T>
    {
        public abstract PriorityCost ComputeHeuristics(T from, T to, int priorityCostLength);
    }
}
