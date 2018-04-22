using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Characters.Actions;
using UnityEngine;

namespace Assets.Scripts.Model
{
    /// <summary>
    /// Edge of the planning graph. Always leads to some interactable object so that it can change the state. 
    /// This edge is an abstraction of the navigation path leading to the interactable object.
    /// </summary>
    public class PlanningEdge : Edge<PlanningNode, PlanningEdgeType>
    {
        private readonly GameObject interactObject;
        private readonly Path<TileNode, TileEdge> path;
        private readonly BaseCharacter character;
        private readonly float interactTime;

        public override float Cost
        {
            get { return path.Cost + interactTime; }
        }

        public int PathLength
        {
            get { return path.Edges.Count; }
        }

        public List<BaseAction> ActionsToComplete
        {
            get
            {
                var result = new List<BaseAction>();
                if (path != null && path.Edges != null && path.Cost < float.MaxValue)
                {
                    result.Add(new NavigationAction(character, path.Edges));
                }

                if (interactObject != null)
                {
                    result.Add(new InteractAction(character, interactObject));
                }

                return result;
            }
        }

        public float SeenTime
        {
            get
            {
                float visibleTime = 0;
                foreach (TileEdge tileEdge in path.Edges)
                {
                    float visibleFraction = tileEdge.Nodes.Count(node => node.IsDetectable(Start.DestroyedDetectors)) /
                                            (float) tileEdge.Nodes.Length;
                    visibleTime += visibleFraction * tileEdge.Cost;
                }

                return visibleTime;
            }
        }

        public float VisibleTime
        {
            get { return path.VisibilityTime; }
        }

        public PlanningEdge(PlanningNode start, PlanningNode neighbor, PlanningEdgeType type, BaseCharacter character,
            Path<TileNode, TileEdge> path, float interactTime, GameObject interactObject = null)
            : base(start, neighbor, type)
        {
            this.path = path;
            this.interactObject = interactObject;
            this.character = character;
            this.interactTime = interactTime;
        }

        public override string ToString()
        {
            return Start + " -> " + Neighbor + " Cost = " + Cost + " Type = " + Type;
        }
    }
}
