using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        public Path<PlanningNode, PlanningEdge>[] PossiblePaths { get; private set; }

        public static readonly int PATHS_COUNT = 6;

        public NavigationGoal(BaseCharacter character, IntegerTuple goalCoordinates) : base(character, goalCoordinates)
        {
            PossiblePaths = new Path<PlanningNode, PlanningEdge>[PATHS_COUNT];
        }

        public override void Activate(PlanningNode startNode=null)
        {
            Character.Log("Goal activated: Navigate to: " + GoalCoordinates);
            StartPlanning(null, startNode);
        }

        private List<List<ClusterNode>> GenerateAllSubsets(List<ClusterNode> nodes)
        {
            var result = new List<List<ClusterNode>>();
            for (int i = 0; i < Mathf.Pow(2, nodes.Count); i++)
            {
                var currentMask = new BitArray(new int[] { i });
                var subset = new List<ClusterNode>();
                for (int j = 0; j < currentMask.Length; j++)
                {
                    if (currentMask[j])
                    {
                        subset.Add(nodes[j]);
                    }
                }
                result.Add(subset);
            }

            return result;
        }

        private List<List<ClusterNode>> GenerateClusterPaths(TileNode startNode, TileNode goalNode)
        {
            List<ClusterNode> contractedGraph = Character.Map.AIModel.ContractedNodes;
            ClusterNode startingCluster = contractedGraph.First(cluster => cluster.Members.Contains(startNode));
            ClusterNode goalCluster = contractedGraph.First(cluster => cluster.Members.Contains(goalNode));
            InitializeGraph(contractedGraph);
            //var result = GenerateAllSubsets(contractedGraph);
            var result = new List<List<ClusterNode>>();

            result.Add(new List<ClusterNode> { startingCluster });
            foreach (ClusterEdge clusterEdge in startingCluster.Edges)
            {
                List<List<ClusterNode>> clusterPaths = GenerateClusterPaths(clusterEdge.Neighbor, goalCluster,
                    new List<ClusterNode> {startingCluster}, new List<ClusterEdge>());
                foreach (List<ClusterNode> clusterPath in clusterPaths)
                {
                    result.Add(clusterPath);
                }
            }

            return result;
        }

        private void InitializeGraph(List<ClusterNode> contractedGraph)
        {
            // Filter clusters
            foreach (ClusterNode cluster in contractedGraph.Copy())
            {
                if (cluster.Members.Count == 1 && cluster.Members.First().Edges.All(edge => edge.IsObstructed(new List<BaseEntity>())))
                {
                    contractedGraph.Remove(cluster);
                }
            }
            // Add edges
            foreach (ClusterNode start in contractedGraph)
            {
                foreach (ClusterNode end in contractedGraph)
                {
                    if (start != end)
                    {
                        foreach (TileNode endMember in end.Members)
                        {
                            if (start.Members.Any(member =>
                                member.Edges.Select(edge => edge.Neighbor).Contains(endMember)))
                            {
                                start.Edges.Add(new ClusterEdge(start, end));
                                break;
                            }
                        }
                    }
                }
            }
        }

        private List<List<ClusterNode>> GenerateClusterPaths(ClusterNode currentCluster, ClusterNode goalCluster,
            List<ClusterNode> traversedNodes, List<ClusterEdge> traversedEdges, int depth = 1)
        {
            traversedNodes.Add(currentCluster);
            if (currentCluster == goalCluster)
            {
                return new List<List<ClusterNode>> {traversedNodes};
            }
            var result = new List<List<ClusterNode>>();
            //if (depth > 5)
            //{
            //    var availableEdges = new List<ClusterEdge>();
            //    foreach (ClusterEdge edge in currentCluster.Edges)
            //    {
            //        if (!traversedEdges.Contains(edge))
            //        {
            //            availableEdges.Add(edge);
            //        }
            //    }

            //    if (availableEdges.Count > 0)
            //    {
            //        ClusterEdge randomEdge = availableEdges.RandomElement();
            //        traversedEdges.Add(randomEdge);
            //        List<List<ClusterNode>> partialResult = GenerateClusterPaths(randomEdge.Neighbor, goalCluster,
            //            traversedNodes.Copy(), traversedEdges.Copy(), depth + 1);
            //        result.AddRange(partialResult);
            //    }
            //}
            //else
            {
                foreach (ClusterEdge edge in currentCluster.Edges)
                {
                    if (!traversedEdges.Contains(edge))
                    {
                        traversedEdges.Add(edge);
                        List<List<ClusterNode>> partialResult = GenerateClusterPaths(edge.Neighbor, goalCluster,
                            traversedNodes.Copy(), traversedEdges.Copy(), depth + 1);
                        result.AddRange(partialResult);
                    }
                }
            }


            return result;
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
                List<List<ClusterNode>> clusterPaths = GenerateClusterPaths(startNode.TileNode, goalNode.TileNode);
                foreach (List<ClusterNode> clusterPath in clusterPaths)
                {
                    startNode.Reset();
                    goalNode.Reset();
                    startNode.UnlockedTileNodes = clusterPath.SelectMany(cluster => cluster.Members);

                    PlanPath(startNode, goalNode, currentMap);

                    // TODO: do not repeat, probably copy node
                    
                }
                TaskManager.Instance.RunOnMainThread(Initialize);
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
            Character.Log("A* time = " + stopwatch.ElapsedMilliseconds / 1000f + " seconds.");
            //TaskManager.Instance.RunOnMainThread(() => Initialize(plannedPath));
            float potentialMeasure = plannedPath.VisibilityMeasure();
            if (potentialMeasure == float.MaxValue)
            {
                return;
            }
            for (int i = 0; i < PossiblePaths.Length; i++)
            {
                if (potentialMeasure <= (float)i / (PossiblePaths.Length - 1) &&
                    (PossiblePaths[i] == null || plannedPath.Cost < PossiblePaths[i].Cost))
                {
                    PossiblePaths[i] = plannedPath;
                }
            }
        }

        private void Initialize()
        {
            foreach (Path<PlanningNode, PlanningEdge> path in PossiblePaths)
            {
                if (path != null)
                {
                    Path = path;
                    break;
                }
            }
            if (Path == null || Path.Cost == float.MaxValue || Path.Edges == null || Path.Edges.Count == 0)
            {
                Character.Log("Planning computation finished. Path NOT found.");
                IsFinished = true;
            }
            else
            {
                Character.Log("Planning computation finished. Path found.");

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
    }
}
