using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Model
{
    public abstract class Edge<TNode, TEdgeType> : IAStarEdge<TNode> where TNode : IAStarNode<TNode>
    {
        public TEdgeType Type;
        public abstract float Cost { get; }

        protected TNode start;
        protected TNode neighbor;

        public TNode Neighbor
        {
            get { return neighbor; }
        }

        public TNode Start
        {
            get { return start; }
        }

        public TNode[] Nodes
        {
            get { return new[] {Start, Neighbor}; }
        }

        public Edge(TNode start, TNode neighbor, TEdgeType type)
        {
            this.start = start;
            this.neighbor = neighbor;
            Type = type;
        }

        public override string ToString()
        {
            return Start + " -> " + Neighbor + " Cost = " + Cost + " Type = " + Type;
        }
    }
}
