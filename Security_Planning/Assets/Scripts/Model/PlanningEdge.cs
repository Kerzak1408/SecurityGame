using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Entities.Characters.Actions;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class PlanningEdge : Edge<PlanningNode, PlanningEdgeType>
    {
        private GameObject interactObject;
        private Path<TileNode, TileEdge> path;
        private BaseCharacter character;
        private float interactTime;

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

        public float VisibilityMeasure
        {
            get
            {
                float visibleTime = 0;
                float totalTime = 0;
                foreach (TileEdge tileEdge in path.Edges)
                {
                    totalTime += tileEdge.Cost;

                    float visibleFraction = tileEdge.Nodes.Count(node =>
                                                node.IsDetectable(Start.DestroyedDetectors,
                                                    character.Data.IgnoredDetectors)) / (float) tileEdge.Nodes.Length;
                    visibleTime += visibleFraction * tileEdge.Cost;
                }

                return visibleTime / totalTime;
            }
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
