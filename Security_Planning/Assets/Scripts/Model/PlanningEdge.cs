using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class PlanningEdge : Edge<PlanningNode, PlanningEdgeType>
    {
        public PlanningEdge(PlanningNode start, PlanningNode neighbor, PlanningEdgeType type, float cost) : base(start, neighbor, type, cost)
        {
        }
    }
}
