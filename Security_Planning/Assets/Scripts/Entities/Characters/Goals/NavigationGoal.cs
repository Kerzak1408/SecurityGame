using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Model;
using UnityEngine;

namespace Entities.Characters.Goals
{
    public class NavigationGoal : BaseGoal
    {
        private IntegerTuple goalCoords;
        private Queue<Edge> path;
        private TileNode followedNode;

        public NavigationGoal(BaseCharacter character, IntegerTuple goalCoords) : base(character)
        {
            this.goalCoords = goalCoords;
        }

        public override void Activate()
        {
            Map currentMap = character.CurrentGame.Map;
            TileNode[,] aiModelTiles = currentMap.AIModel.Tiles;
            TileNode startNode = currentMap.GetClosestTile(character.transform.position);
            List<Edge> pathList = AStarAlgorithm.AStar<TileNode, Edge>(
                startNode, 
                aiModelTiles[goalCoords.First, goalCoords.Second],
                new EuclideanHeuristics(currentMap.Tiles), 
                Debug.Log, 
                node => node.IsDetectable(),
                edge => edge.Type == EdgeType.CARD_DOOR || edge.Type == EdgeType.KEY_DOOR).Edges;
            if (pathList == null)
            {
                IsCompleted = true;
                return;
            }
            path = new Queue<Edge>(pathList);
        }

        public override void Update()
        {
            if (IsCompleted) return;
            if (followedNode == null || character.NavigateTo(followedNode))
            {
                if (path.Count > 0)
                {
                    Edge currentEdge = path.Dequeue();
                    followedNode = currentEdge.Neighbor;
                }
                else
                {
                    IsCompleted = true;
                }
            }
        }
    }
}