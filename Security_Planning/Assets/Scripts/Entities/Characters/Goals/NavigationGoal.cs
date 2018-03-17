using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using UnityEngine;

namespace Entities.Characters.Goals
{
    public class NavigationGoal : BaseGoal
    {
        private IntegerTuple goalCoords;
        private Queue<TileNode> path;
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
            List<TileNode> pathList = AStarAlgorithm.AStar(
                startNode, 
                aiModelTiles[goalCoords.First, goalCoords.Second],
                new EuclideanHeuristics(currentMap.Tiles), 
                Debug.Log, 
                node => node.IsDetectable());
            path = new Queue<TileNode>(pathList);
        }

        public override void Update()
        {
            if (followedNode == null || character.NavigateTo(followedNode))
            {
                if (path.Count > 0)
                {
                    followedNode = path.Dequeue();
                }
                else
                {
                    IsCompleted = true;
                }
            }
        }
    }
}