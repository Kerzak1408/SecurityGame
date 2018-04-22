using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters.Actions;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities.Characters.Behaviours
{
    /// <summary>
    /// Collects as many money as possible. The order of collection is determined by the proximity od character to the pile of money.
    /// </summary>
    public class CollectEverythingBehaviour : BaseBehaviour
    {
        private Queue<BaseGoal> goals;
        private BaseAction currentAction;
        private Queue<BaseAction> actionsQueue;
        
        
        public int TotalMoneyGoals { get; private set; }
        public int SuccessfulMoneyGoals { get; private set; }

        public CollectEverythingBehaviour(BaseCharacter character) : base(character)
        {

        }

        public override void Start()
        {
            goals = GenerateGoals(Character);
            TotalMoneyGoals = goals.Count(goal => goal is MoneyGoal);
            Character.StartCoroutine(PlanGoals(result =>
            {
                IsInitialized = true;
                actionsQueue = result;
            }));
        }

        /// <summary>
        /// Generate all money goals. Order them by the distance to <paramref name="character"/>.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="navigateBack">Should the result contain Navigation goal to the start position?</param>
        /// <returns></returns>
        public static Queue<BaseGoal> GenerateGoals(BaseCharacter character, bool navigateBack=true)
        {
            Map currentMap = character.Map;
            var result = new Queue<BaseGoal>();
            IEnumerable<GameObject> moneyEntities = currentMap.Entities.Where(go => go.HasScriptOfType<MoneyEntity>());
            IOrderedEnumerable<GameObject> orderedEntities = moneyEntities.OrderBy(
                entity => Vector3.Distance(character.transform.position, entity.transform.position));
            foreach (GameObject moneyObject in orderedEntities)
            {
                TileNode closestTile = currentMap.GetClosestTile(moneyObject.transform.position);
                result.Enqueue(new MoneyGoal(character, closestTile.Position, moneyObject));
            }

            if (navigateBack)
            {
                result.Enqueue(new NavigationGoal(character, character.Position));
            }
            return result;
        }

        /// <summary>
        /// Plan the action needed to complete all the goals.
        /// </summary>
        /// <param name="resultAction">What to do with the planned actions.</param>
        /// <returns></returns>
        private IEnumerator PlanGoals(Action<Queue<BaseAction>> resultAction)
        {
            var allActions = new Queue<BaseAction>();
            PlanningNode startNode = null;
            while (goals.Count > 0)
            {
                NavigationGoal goal = goals.Dequeue() as NavigationGoal;
                goal.Reset();
                if (startNode != null)
                {
                    startNode.Reset();
                }

                goal.MaxVisibility = Character.Data.MaxVisibilityMeasure;
                goal.Activate(startNode);

                while (!goal.IsInitialized)
                {
                    yield return null;
                }

                var currentPath = goal.Path;
                if (currentPath.GoalNode != null)
                {
                    startNode = currentPath.GoalNode.Copy();
                }

                if (currentPath.Edges == null)
                {
                    continue;
                }

                foreach (PlanningEdge planningEdge in currentPath.Edges)
                {
                    foreach (BaseAction action in planningEdge.ActionsToComplete)
                    {
                        if (action.GetType() == typeof(InteractAction))
                        {
                            var interactAction = (InteractAction) action;
                            interactAction.InteractedName = interactAction.Interacted.name;
                        }

                        allActions.Enqueue(action);
                    }
                }
            }

            resultAction(allActions);
        }

        public override void Update()
        {
            if (currentAction == null || currentAction.IsCompleted)
            {
                if (actionsQueue.Count > 0)
                {
                    currentAction = actionsQueue.Dequeue();
                    currentAction.Activate();
                }
            }
            currentAction.Update();
        }
    }
}
