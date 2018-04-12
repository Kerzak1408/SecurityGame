using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class ClusterEdge : IAStarEdge<ClusterNode>
    {
        public ClusterNode Start { get; private set; }
        public ClusterNode Neighbor { get; private set; }
        public float Cost { get; private set; }

        public ClusterEdge(ClusterNode start, ClusterNode neighbor)
        {
            Start = start;
            Neighbor = neighbor;
        }

        public override string ToString()
        {
            return Start + " -> " + Neighbor + " Cost = " + Cost;
        }
    }
}
