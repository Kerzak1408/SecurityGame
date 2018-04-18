using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Algorithms.AStar.Heuristics
{
    public class TrivialHeuristics<T> : Heuristics<T> where T : IAStarNode<T>
    {
        public override PriorityCost ComputeHeuristics(T from, T to, int priorityCostLength)
        {
            return new PriorityCost(priorityCostLength, 0);
        }
    }
}
