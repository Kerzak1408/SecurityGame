using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using Entities.Characters.Goals;
using UnityEngine;

namespace Assets.Scripts.Entities.Characters.Goals
{
    public class NavigationGoal : BaseGoal
    {
        private BaseAction currentAction;
        private Queue<BaseAction> actions;
        private bool isInitialized;

        public NavigationGoal(BaseCharacter character, IntegerTuple goalCoordinates) : base(character, goalCoordinates)
        {
        }

        public override void Activate()
        {
            Character.Log("Goal activated: Navigate to: " + GoalCoordinates);
            StartPlanning(null);
        }

        protected void StartPlanning(GameObject finalObject)
        {
            Map currentMap = Character.CurrentGame.Map;
            PlanningNode startNode, goalNode;
            currentMap.GetPlanningModel(Character, GoalCoordinates, finalObject, out startNode, out goalNode);
            Character.Log("Planning started.");
            Thread planningThread = new Thread(() => PlanPath(startNode, goalNode, currentMap));
            planningThread.Start();
        }

        private void PlanPath(PlanningNode startNode, PlanningNode goalNode, Map currentMap)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Path<PlanningNode, PlanningEdge> plannedPath = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(
                startNode,
                goalNode,
                new EuclideanHeuristics<PlanningNode>(currentMap.Tiles),
                edgeFilter: edge => Character.Data.ForbiddenPlanningEdgeTypes.Contains(edge.Type));
            stopwatch.Stop();
            Character.Log("A* time = " + stopwatch.ElapsedMilliseconds / 1000f + " seconds.");
            TaskManager.Instance.RunOnMainThread(() => Initialize(plannedPath));
        }

        private void Initialize(Path<PlanningNode, PlanningEdge> plannedPath)
        {
            if (plannedPath.Cost == float.MaxValue || plannedPath.Edges == null)
            {
                Character.Log("Planning computation finished. Path NOT found.");
                IsFinished = true;
            }
            else
            {
                Character.Log("Planning computation finished. Path found.");
                actions = new Queue<BaseAction>();
                foreach (PlanningEdge planningEdge in plannedPath.Edges)
                {
                    List<BaseAction> edgeActions = planningEdge.ActionsToComplete;
                    foreach (BaseAction edgeAction in edgeActions)
                    {
                        actions.Enqueue(edgeAction);
                    }
                }
            }

            isInitialized = true;
        }

        public override void Update()
        {
            if (!isInitialized || IsFinished) return;
            if (currentAction == null || currentAction.IsCompleted)
            {
                if (actions.Count > 0)
                {
                    currentAction = actions.Dequeue();
                    currentAction.Activate();
                }
                else
                {
                    IsFinished = true;
                }
            }
            currentAction.Update();
        }
    }
}
