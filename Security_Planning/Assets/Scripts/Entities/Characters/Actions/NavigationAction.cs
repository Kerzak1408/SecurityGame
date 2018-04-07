using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Model;
using UnityEngine;

namespace Entities.Characters.Actions
{
    public class NavigationAction : BaseAction
    {
        private IntegerTuple goalCoords;
        private TileEdge currentEdge;

        public Queue<TileEdge> PathQueue { get; private set; }

        public NavigationAction(BaseCharacter character, List<TileEdge> navigationEdges) : base(character)
        {
            if (navigationEdges == null || navigationEdges.Count == 0)
            {
                IsCompleted = true;
            }
            else
            {
                goalCoords = navigationEdges.Last().Neighbor.Position;
                PathQueue = new Queue<TileEdge>(navigationEdges);
            }
        }

        public override void Activate()
        {
            character.Log("Navigation to " + goalCoords + " started.");
        }

        public override void Update()
        {
            if (IsCompleted) return;
            if (currentEdge == null || (currentEdge.IsOpen && character.NavigateTo(currentEdge.Neighbor)))
            {
                if (PathQueue.Count > 0)
                {
                    currentEdge = PathQueue.Dequeue();
                    character.Log("Next target: " + currentEdge.Neighbor.Position + " of type " + currentEdge.Type + ".");
                    if (!currentEdge.IsOpen)
                    {
                        currentEdge.Open(character);
                    }
                }
                else
                {
                    IsCompleted = true;
                    character.Log("Navigation to " + goalCoords + " completed.");
                }
            }
        }
    }
}