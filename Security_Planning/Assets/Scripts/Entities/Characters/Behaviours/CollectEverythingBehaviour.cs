using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters.Goals;
using Entities.Characters.Goals;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Entities.Characters.Behaviours
{
    public class CollectEverythingBehaviour : BaseBehaviour
    {
        private Queue<BaseGoal> goals;
        private BaseGoal currentGoal;

        public CollectEverythingBehaviour(BaseCharacter character) : base(character)
        {
        }

        public override void Start()
        {
            Map currentMap = character.CurrentGame.Map;
            goals = new Queue<BaseGoal>();
            IEnumerable<GameObject> moneyEntities = currentMap.Entities.Where(go => go.HasScriptOfType<MoneyEntity>());
            IOrderedEnumerable<GameObject> orderedEntities = moneyEntities.OrderBy(
                entity => Vector3.Distance(character.transform.position, entity.transform.position));
            foreach (GameObject moneyObject in orderedEntities)
            {
                TileNode closestTile = currentMap.GetClosestTile(moneyObject.transform.position);
                goals.Enqueue(new MoneyGoal(character, closestTile.Position, moneyObject));
            }
            goals.Enqueue(new NavigationGoal(character, character.Position));
        }

        public override void Update()
        {
            if (currentGoal == null || currentGoal.IsFinished)
            {
                if (goals.Count > 0)
                {
                    currentGoal = goals.Dequeue();
                    currentGoal.Activate();
                }
                else
                {
                    currentGoal = null;
                    return;
                }
            }
            currentGoal.Update();
            //if (followedNode != null)
            //{
            //    if (character.NavigateTo(followedNode))
            //    {
            //        if (waitingForNextNode)
            //        {
            //            followedNode = path.Count == 0 ? null : path.Dequeue();
            //        }
            //    }
            //    else
            //    {
            //        waitingForNextNode = true;
            //    }
            //}
            //else
            //{
            //    RecomputePath();
            //}
        }

        //private void RecomputePath()
        //{
        //    Map currentMap = character.CurrentGame.Map;
        //    aiModelTiles = currentMap.AIModel.Tiles;
        //    TileNode startNode = followedNode == null ? currentMap.GetClosestTile(character.transform.position) : 
        //        aiModelTiles[followedNode.Position.First, followedNode.Position.Second];
        //    List<TileNode> fullPath = AStarAlgorithm.AStar(startNode, aiModelTiles[currentGoal.First, currentGoal.Second], heuristics, Debug.Log, node => node.IsDetectable());
        //    if (fullPath == null || fullPath.Count <= 1)
        //    {
        //        currentGoal = goals.Count == 0 ? null : goals.Dequeue();
        //        followedNode = null;
        //    }
        //    else
        //    {
        //        path = new Queue<TileNode>(fullPath);
        //        followedNode = path.Dequeue();
        //    }

        //}
    }
}
