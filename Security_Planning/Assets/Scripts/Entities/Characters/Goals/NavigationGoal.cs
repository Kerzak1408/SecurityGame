using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Assets.Scripts.Algorithms.AStar.Heuristics;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Extensions;
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
        public float MaxVisibility { get; set; }

        public static readonly int PATHS_COUNT = 6;

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
                TaskManager.Instance.RunOnMainThread(Initialize);
            });
            planningThread.Start();

        }

        private void PlanPath(PlanningNode startNode, PlanningNode goalNode, Map currentMap)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Character.Map.AIModel.Reset();
            startNode.IsVisibilityPriority = true;
            Path<PlanningNode, PlanningEdge> leastSeenPath = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(
                startNode,
                goalNode,
                new EuclideanHeuristics<PlanningNode>(currentMap.Tiles, 1),
                edgeFilter: edge => Character.Data.ForbiddenPlanningEdgeTypes.Contains(edge.Type),
                computeCost: GetCostFunction(true));

            float longestPathLength = leastSeenPath.Cost;
            float longestPathVisibility = leastSeenPath.VisibleTime();
            startNode.Reset();
            Character.Map.AIModel.Reset();
            startNode.IsVisibilityPriority = false;

            UnityEngine.Debug.Log(
                "------------------------------------SHORTEST PATH--------------------------------------------------");
            Path<PlanningNode, PlanningEdge> shortestPath = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(
                startNode,
                goalNode,
                new EuclideanHeuristics<PlanningNode>(currentMap.Tiles, 0),
                edgeFilter: edge => Character.Data.ForbiddenPlanningEdgeTypes.Contains(edge.Type),
                computeCost: GetCostFunction(false)
                );

            float shortestPathVisibility = shortestPath.VisibleTime();
            //if (MaxVisibility == 0)
            //{
            //    Path = leastSeenPath;
            //}
            //else if (MaxVisibility == 1)
            //{
            //    Path = shortestPath;
            //}
            //else
            {
                startNode.Reset();
                Character.Map.AIModel.Reset();
                startNode.UseVisibilityLimit(longestPathVisibility + MaxVisibility * (shortestPathVisibility - longestPathVisibility));
                //Path = shortestPath;
                Path = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(
                    startNode,
                    goalNode,
                    new EuclideanHeuristics<PlanningNode>(currentMap.Tiles, 0),
                    edgeFilter: edge => Character.Data.ForbiddenPlanningEdgeTypes.Contains(edge.Type),
                    onBeforeAddToOpen: edge =>
                    {
                        edge.Neighbor.VisibleTime = edge.Start.VisibleTime + edge.VisibleTime;
                        edge.Neighbor.TotalTime = edge.Start.TotalTime + edge.Cost;
                        edge.Neighbor.UseVisibilityLimit(longestPathVisibility + MaxVisibility * (shortestPath.VisibleTime() - longestPathVisibility));
                    },
                    computeCost: GetCostFunction(false)
                );
            }

            //UnityEngine.Debug.Log("Path visibility = " + Path.VisibleTime() + " max visibility = " + (leastSeenPath.VisibleTime() + MaxVisibility * (shortestPath.VisibleTime() - leastSeenPath.VisibleTime())) + 
            //" path length = " + Path.Cost);
            stopwatch.Stop();
            Character.Log("A* time = " + stopwatch.ElapsedMilliseconds / 1000f + " seconds.");
        }

        private Func<PlanningEdge, PriorityCost> GetCostFunction(bool isVisibilityPriority)
        {
            return edge =>
            {
                var result = new PriorityCost(2);
                result.AddCost(isVisibilityPriority ? 0 : 1, edge.VisibleTime);
                result.AddCost(isVisibilityPriority ? 1 : 0, edge.Cost);
                return result;
            };
        }

        private void Initialize()
        {
            if (Path == null || Path.Edges == null || Path.Edges.Count == 0)
            {
                Character.Log("Planning computation finished. Path NOT found.");
                IsFinished = true;
                IsSuccessFul = false;
            }
            else
            {
                Character.Log("Planning computation finished. Path found.");
                IsSuccessFul = true;
                PlanningEdge lastEdge = Path.Edges.Last();
                PlanningNode lastNode = lastEdge.Neighbor;
                PlanningNode secondLastNode = lastEdge.Start;
                lastNode.DestroyedDetectors = secondLastNode.DestroyedDetectors.Copy();
                lastNode.DestroyedObstacles = secondLastNode.DestroyedObstacles.Copy();
                lastNode.UnlockedEdges = secondLastNode.UnlockedEdges.Copy();

                GoalNode = Path.Edges.Last().Neighbor;
                actions = new Queue<BaseAction>();
                foreach (PlanningEdge planningEdge in Path.Edges)
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

        public void Reset()
        {
            IsInitialized = false;
            Path = null;
        }
    }
}
