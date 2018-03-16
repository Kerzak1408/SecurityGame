using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.MapEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Entities.Characters.Behaviours
{
    public class CollectEverythingBehaviour : BaseBehaviour
    {
        private Queue<TileNode> path;
        private TileNode followedNode;
        private EuclideanHeuristics heuristics;
        private TileNode[,] aiModelTiles;
        private Queue<IntegerTuple> goals;
        private IntegerTuple currentGoal;
        private bool waitingForNextNode = true;

        public CollectEverythingBehaviour(BaseCharacter character) : base(character)
        {
        }

        public override void Start()
        {
            Map currentMap = character.CurrentGame.Map;
            heuristics = new EuclideanHeuristics(currentMap.Tiles);
            aiModelTiles = currentMap.AIModel.Tiles;
            goals = new Queue<IntegerTuple>();
            IEnumerable<GameObject> moneyEntities = currentMap.Entities.Where(go => go.HasScriptOfType<MoneyEntity>());
            foreach (GameObject moneyObject in moneyEntities)
            {
                TileNode closestTile = currentMap.GetClosestTile(moneyObject.transform.position);
                goals.Enqueue(closestTile.Position);
            }
        }

        public override void Update()
        {
            if (currentGoal == null)
            {
                if (goals.Count > 0) currentGoal = goals.Dequeue();
                else return;
            }
            if (followedNode != null)
            {
                if (character.NavigateTo(followedNode))
                {
                    if (waitingForNextNode)
                    {
                        //RecomputePath();
                        followedNode = path.Count == 0 ? null : path.Dequeue();
                    }
                }
                else
                {
                    waitingForNextNode = true;
                }
            }
            else
            {
                RecomputePath();
            }
        }

        private void RecomputePath()
        {
            Map currentMap = character.CurrentGame.Map;
            //currentMap.ExtractAIModel();
            aiModelTiles = currentMap.AIModel.Tiles;
            TileNode startNode = followedNode == null ? currentMap.GetClosestTile(character.transform.position) : 
                aiModelTiles[followedNode.Position.First, followedNode.Position.Second];
            List<TileNode> fullPath = AStarAlgorithm.AStar(startNode, aiModelTiles[currentGoal.First, currentGoal.Second], heuristics, Debug.Log, node => node.IsDetectable());
            if (fullPath == null || fullPath.Count <= 1)
            {
                currentGoal = goals.Count == 0 ? null : goals.Dequeue();
                followedNode = null;
            }
            else
            {
                path = new Queue<TileNode>(fullPath);
                followedNode = path.Dequeue();
            }

        }
    }
}
