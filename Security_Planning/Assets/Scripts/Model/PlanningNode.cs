using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Items;

namespace Assets.Scripts.Model
{
    public class PlanningNode : IAStarNode<PlanningNode>
    {
        public IntegerTuple Position { get; set; }
        public List<EdgeType> UnlockedEdges;

        public List<IAStarEdge<PlanningNode>> Edges { get; private set; }

        public PlanningNode(IntegerTuple position, List<EdgeType> unlockedEdges)
        {
            Edges = new List<IAStarEdge<PlanningNode>>();
            Position = position;
            UnlockedEdges = unlockedEdges;
        }

        public void AddEdge(PlanningEdge edge)
        {
            Edges.Add(edge);
        }
    }
}
