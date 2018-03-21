using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Model;
using UnityEngine;

namespace Entities.Characters.Actions
{
    public class NavigationAction : BaseAction
    {
        private IntegerTuple goalCoords;
        private Queue<TileEdge> pathQueue;
        private TileNode followedNode;

        public NavigationAction(BaseCharacter character, List<TileEdge> navigationEdges) : base(character)
        {
            pathQueue = new Queue<TileEdge>(navigationEdges);
        }

        public override void Activate()
        {

        }

        public override void Update()
        {
            if (IsCompleted) return;
            if (followedNode == null || character.NavigateTo(followedNode))
            {
                if (pathQueue.Count > 0)
                {
                    TileEdge currentEdge = pathQueue.Dequeue();
                    followedNode = currentEdge.Neighbor;
                    switch (currentEdge.Type)
                    {
                        case EdgeType.KEY_DOOR:
                            currentEdge.Interactable.Interact(character);
                            break;
                    }
                }
                else
                {
                    IsCompleted = true;
                }
            }
        }
    }
}