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
        private TileEdge currentEdge;

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
            if (currentEdge == null || (currentEdge.IsOpen && character.NavigateTo(currentEdge.Neighbor)))
            {
                if (pathQueue.Count > 0)
                {
                    currentEdge = pathQueue.Dequeue();
                    if (!currentEdge.IsOpen)
                    {
                        currentEdge.Open(character);
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