using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataStructures
{
    public class Path<TNode, TEdge> where TEdge : IAStarEdge<TNode> where TNode : IAStarNode<TNode>
    {
        public List<TEdge> Edges;
        public float Cost;

        public Path()
        {
            Edges = new List<TEdge>();
        }

        public Path(List<TEdge> edges, float cost)
        {
            Edges = edges;
            Cost = cost;
        }

        public void AddEdgeToBeginning(TEdge edge)
        {
            Edges.Insert(0, edge);
            Cost += edge.Cost;
        }
    }
}
