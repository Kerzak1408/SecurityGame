using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Items;

namespace Assets.Scripts.Model
{
    public class PlanningNode : IAStarNode<PlanningNode>
    {
        public IntegerTuple Position { get; set; }
        public List<EdgeType> UnlockedEdges { get; set; }
        public List<IObstacle> DestroyedObstacles { get; set; }
        public List<DetectorEntity> DestroyedDetectors { get; set; }

        public List<IAStarEdge<PlanningNode>> Edges { get; private set; }

        public PlanningNode(IntegerTuple position, List<EdgeType> unlockedEdges, 
            List<IObstacle> destroyedObstacles = null, List<DetectorEntity> destroyedDetectors = null)
        {
            Edges = new List<IAStarEdge<PlanningNode>>();
            Position = position;
            UnlockedEdges = unlockedEdges;
            DestroyedObstacles = destroyedObstacles ?? new List<IObstacle>();
            DestroyedDetectors = destroyedDetectors ?? new List<DetectorEntity>();
        }

        public void AddEdge(PlanningEdge edge)
        {
            Edges.Add(edge);
        }
    }
}
