using System;
using System.Collections;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters.Goals;
using Entities.Characters.Goals;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using UnityEngine;

namespace Entities.Characters.Behaviours
{
    public class CollectEverythingBehaviour : BaseBehaviour
    {
        private Queue<BaseGoal> goals;
        //private BaseGoal currentGoal;
        private BaseAction currentAction;
        private Queue<BaseAction> actionsQueue;
        
        
        public int TotalMoneyGoals { get; private set; }
        public int SuccessfulMoneyGoals { get; private set; }

        public CollectEverythingBehaviour(BaseCharacter character) : base(character)
        {

        }

        public override void Start()
        {
            goals = GenerateGoals(character);
            TotalMoneyGoals = goals.Count(goal => goal is MoneyGoal);
            character.StartCoroutine(PlanGoals(result =>
            {
                IsInitialized = true;
                actionsQueue = result;
            }));
        }

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

                goal.MaxVisibility = character.Data.MaxVisibilityMeasure;
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

        private IEnumerator WaitForGoalPlanning(Action<List<BaseAction>[]> resultAction)
        {
            var actionsToDraw = new List<BaseAction>[NavigationGoal.PATHS_COUNT];
            for (int i = 0; i < actionsToDraw.Length; i++)
            {
                actionsToDraw[i] = new List<BaseAction>();
            }

            Queue<BaseGoal> goals = CollectEverythingBehaviour.GenerateGoals(character, false);
            PlanningNode[] startNodes = new PlanningNode[actionsToDraw.Length];
            while (goals.Count > 0)
            {

                NavigationGoal goal = goals.Dequeue() as NavigationGoal;

                for (int i = 0; i < startNodes.Length; i++)
                //for (int i = 0; i < 1; i++)
                {
                    goal.Reset();
                    PlanningNode startNode = startNodes[i];
                    if (startNode != null)
                    {
                        startNode.Reset();
                    }
                    Path<PlanningNode, PlanningEdge> currentPath;

                    goal.MaxVisibility = (float)i / (startNodes.Length - 1);
                    //goal.MaxVisibility = 1.0f;
                    goal.Activate(startNode);

                    while (!goal.IsInitialized)
                    {
                        yield return null;
                    }
                    currentPath = goal.Path;
                    if (currentPath.GoalNode != null)
                    {
                        startNodes[i] = currentPath.GoalNode.Copy();
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
                                var interactAction = (InteractAction)action;
                                interactAction.InteractedName = interactAction.Interacted.name;
                            }
                            actionsToDraw[i].Add(action);
                        }
                    }
                }
            }
            resultAction(actionsToDraw);
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

            //if (currentGoal == null || currentGoal.IsFinished)
            //{
            //    if (currentGoal is MoneyGoal && currentGoal.IsSuccessFul)
            //    {
            //        SuccessfulMoneyGoals++;
            //    }
            //    if (goals.Count > 0)
            //    {
            //        currentGoal = goals.Dequeue();
            //        currentGoal.Activate();
            //    }
            //    else
            //    {
            //        character.GoalsCompleted();
            //        currentGoal = null;
            //        return;
            //    }
            //}
            //currentGoal.Update();
        }
    }
}
