﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;

namespace Assets.Scripts.Entities.Characters.Actions
{
    public class NavigationAction : BaseAction
    {
        private readonly IntegerTuple goalCoords;
        private TileEdge currentEdge;

        public Queue<TileEdge> PathQueue { get; private set; }

        public NavigationAction(BaseCharacter character, ICollection<TileEdge> navigationEdges) : base(character)
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
            Character.Log("Navigation to " + goalCoords + " started.");
        }

        public override void Update()
        {
            if (IsCompleted) return;
            if (currentEdge == null || (currentEdge.IsOpen && Character.NavigateTo(currentEdge.Neighbor)))
            {
                if (PathQueue.Count > 0)
                {
                    currentEdge = PathQueue.Dequeue();
                    Character.Log("Next target: " + currentEdge.Neighbor.Position + " of type " + currentEdge.Type + ".");
                    if (!currentEdge.IsOpen)
                    {
                        currentEdge.Open(Character);
                    }
                }
                else
                {
                    IsCompleted = true;
                    Character.Log("Navigation to " + goalCoords + " completed.");
                }
            }
        }
    }
}