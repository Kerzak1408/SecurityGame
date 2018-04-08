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
        public bool IsInitialized { get; private set; }
        public PlanningNode GoalNode { get; private set; }
        public Path<PlanningNode, PlanningEdge> Path { get; private set; }

        public NavigationGoal(BaseCharacter character, IntegerTuple goalCoordinates) : base(character, goalCoordinates)
        {
        }

        public override void Activate(PlanningNode startNode=null)
        {
            Character.Log("Goal activated: Navigate to: " + GoalCoordinates);
            StartPlanning(null, startNode);
        }

        protected void StartPlanning(GameObject finalObject, PlanningNode startOverrideNode=null)
        {
            Map currentMap = Character.Map;
            PlanningNode startNode, goalNode;
            currentMap.GetPlanningModel(Character, GoalCoordinates, finalObject, out startNode, out goalNode);
            if (startOverrideNode != null)
            {
                startOverrideNode.CreatorsDictionary = startNode.CreatorsDictionary;
                startOverrideNode.FiniteObject = startNode.FiniteObject;
                startNode = startOverrideNode;
                startNode.GoalNode = goalNode;
            }
            Character.Log("Planning started.");
            Thread planningThread = new Thread(() =>
            {
                PlanPath(startNode, goalNode, currentMap);
            });
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
            //Character.Log("A* time = " + stopwatch.ElapsedMilliseconds / 1000f + " seconds.");
            //TaskManager.Instance.RunOnMainThread(() => Initialize(plannedPath));
            Initialize(plannedPath);
        }

        private void Initialize(Path<PlanningNode, PlanningEdge> plannedPath)
        {
            Path = plannedPath;
            if (plannedPath.Cost == float.MaxValue || plannedPath.Edges == null || plannedPath.Edges.Count == 0)
            {
                Character.Log("Planning computation finished. Path NOT found.");
                IsFinished = true;
            }
            else
            {
                Character.Log("Planning computation finished. Path found.");

                PlanningEdge lastEdge = plannedPath.Edges.Last();
                PlanningNode lastNode = lastEdge.Neighbor;
                PlanningNode secondLastNode = lastEdge.Start;
                lastNode.DestroyedDetectors = secondLastNode.DestroyedDetectors.Copy();
                lastNode.DestroyedObstacles = secondLastNode.DestroyedObstacles.Copy();
                lastNode.UnlockedEdges = secondLastNode.UnlockedEdges.Copy();
                
                GoalNode = plannedPath.Edges.Last().Neighbor;
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
            IsInitialized = true;
        }

        public override void Update()
        {
            if (!IsInitialized || IsFinished) return;
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
