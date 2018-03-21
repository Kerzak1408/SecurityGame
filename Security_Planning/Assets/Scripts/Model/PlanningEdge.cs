using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Entities.Characters.Actions;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class PlanningEdge : Edge<PlanningNode, PlanningEdgeType>
    {
        private GameObject interactObject;
        private Path<TileNode, TileEdge> path;
        private BaseCharacter character;

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

        public PlanningEdge(PlanningNode start, PlanningNode neighbor, PlanningEdgeType type, BaseCharacter character,
            Path<TileNode, TileEdge> path, GameObject interactObject = null)
            : base(start, neighbor, type, path.Cost)
        {
            this.path = path;
            this.interactObject = interactObject;
            this.character = character;
        }
    }
}
