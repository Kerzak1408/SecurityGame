using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.Algorithms.AStar.Heuristics
{
    /// <summary>
    /// Computes the heuristics as the euclidean distance between from.Position and to.Position.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class EuclideanHeuristics<TNode> : Heuristics<TNode> where TNode : IAStarNode<TNode>
    {
        private readonly int pathLengthIndex;

        public EuclideanHeuristics(int pathLengthIndex)
        {
            this.pathLengthIndex = pathLengthIndex;
        }

        public override PriorityCost ComputeHeuristics(TNode from, TNode to, int priorityCostLength)
        {
            Vector3 fromPosition = new Vector3(from.Position.First, from.Position.Second);
            Vector3 toPosition = new Vector3(to.Position.First, to.Position.Second);
            var result = new PriorityCost(priorityCostLength);
            result[pathLengthIndex] = Vector3.Distance(fromPosition, toPosition);
            return result;
        }
    }
}