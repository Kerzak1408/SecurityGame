using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Algorithms.AStar.Interfaces;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Path is the list of the edges.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public class Path<TNode, TEdge> where TEdge : IAStarEdge<TNode> where TNode : IAStarNode<TNode>
    {
        public List<TEdge> Edges { get; set; }
        public float Cost { get; set; }
        /// <summary>
        /// How long would the character be visible if he took this path.
        /// </summary>
        public float VisibilityTime { get; set; }

        public Path()
        {
            Edges = new List<TEdge>();
        }

        public Path(List<TEdge> edges, float cost)
        {
            Edges = edges;
            Cost = cost;
        }

        public TNode GoalNode
        {
            get
            {
                return Edges == null ? default(TNode) : Edges.Last().Neighbor;
            }
        }

        public void AddEdgeToBeginning(TEdge edge)
        {
            Edges.Insert(0, edge);
            Cost += edge.Cost;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Cost: " + Cost);
            foreach (TEdge edge in Edges)
            {
                stringBuilder.Append(" {").Append(edge).Append("} ");
            }
            return stringBuilder.ToString();
        }
    }
}
