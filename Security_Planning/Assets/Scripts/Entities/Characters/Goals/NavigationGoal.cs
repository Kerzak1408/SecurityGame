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
        private Queue<TileEdge> path;
        private TileNode followedNode;

        public NavigationGoal(BaseCharacter character, IntegerTuple goalCoords) : base(character)
        {
            this.goalCoords = goalCoords;
        }

        public override void Activate()
        {
            Map currentMap = character.CurrentGame.Map;
            PlanningNode startNode, goalNode;
            currentMap.GetPlanningModel(character, goalCoords, out startNode, out goalNode);
            Path<PlanningNode, PlanningEdge> plannedPath = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(startNode,
                goalNode,
                new EuclideanHeuristics<PlanningNode>(currentMap.Tiles), Debug.Log);
            Debug.Log("Planned path cost = " + plannedPath.Cost);
            foreach (PlanningEdge plannedEdge in plannedPath.Edges)
            {
                Debug.Log(plannedEdge);
            }

            //TileNode[,] aiModelTiles = currentMap.AIModel.Tiles;
            //TileNode startNode = currentMap.GetClosestTile(character.transform.position);
            //List<TileEdge> pathList = AStarAlgorithm.AStar<TileNode, TileEdge>(
            //    startNode, 
            //    aiModelTiles[goalCoords.First, goalCoords.Second],
            //    new EuclideanHeuristics(currentMap.Tiles), 
            //    Debug.Log, 
            //    node => node.IsDetectable(),
            //    edge => edge.Type == EdgeType.CARD_DOOR || edge.Type == EdgeType.KEY_DOOR).Edges;
            //if (pathList == null)
            //{
            //    IsCompleted = true;
            //    return;
            //}
            //path = new Queue<TileEdge>(pathList);
        }

        public override void Update()
        {
            if (IsCompleted) return;
            if (followedNode == null || character.NavigateTo(followedNode))
            {
                if (path.Count > 0)
                {
                    TileEdge currentEdge = path.Dequeue();
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