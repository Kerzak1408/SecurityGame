using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class Edge<TNode, TEdgeType> : IAStarEdge<TNode> where TNode : IAStarNode<TNode>
    {
        public TEdgeType Type;
        public float Cost { get; private set; }

        private TNode start;
        private TNode neighbor;

        public TNode Neighbor
        {
            get { return neighbor; }
        }

        public TNode Start
        {
            get { return start; }
        }

        public Edge(TNode start, TNode neighbor, TEdgeType type, float cost)
        {
            this.start = start;
            this.neighbor = neighbor;
            Type = type;
            Cost = cost;
        }

        public override string ToString()
        {
            return string.Join(", ", new string[] { Type.ToString(), Cost.ToString() });
        }
    }
}
