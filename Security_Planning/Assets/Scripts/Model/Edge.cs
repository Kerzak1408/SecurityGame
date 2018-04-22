using Assets.Scripts.Algorithms.AStar.Interfaces;

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

        protected Edge(TNode start, TNode neighbor, TEdgeType type)
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
