using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Algorithms.AStar.Heuristics;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class PlanningNode : IAStarNode<PlanningNode>
    {
        public TileNode TileNode { get; set; }
        public IntegerTuple Position { get; set; }
        public List<EdgeType> UnlockedEdges { get; set; }
        public List<IObstacle> DestroyedObstacles { get; set; }
        public List<DetectorEntity> DestroyedDetectors { get; set; }
        public PlanningNode GoalNode { get; set; }

        public Dictionary<IPlanningEdgeCreator, List<TileNode>> CreatorsDictionary { get; set; }
        private BaseCharacter character;
        private List<IAStarEdge<PlanningNode>> edges;
        private bool useVisibilityLimit;
        private float maxAbsoluteVisibility;

        public GameObject FiniteObject { get; set; }
        public bool UsePriorityCost { get; set; }
        public float VisibleTime { get; set; }
        public float TotalTime { get; set; }


        public List<IAStarEdge<PlanningNode>> Edges
        {
            get
            {
                if (edges == null)
                {
                    edges = new List<IAStarEdge<PlanningNode>>();
                    Func<TileEdge, PriorityCost> computeCost = edge => ComputeEdgeCost(edge, DestroyedObstacles);
                    Heuristics<TileNode> heuristics = new EuclideanHeuristics<TileNode>(character.Map.Tiles);
                    //heuristics = new TrivialHeuristics<TileNode>();
                    if (UsePriorityCost)
                    {
                        heuristics = new TrivialHeuristics<TileNode>();
                        computeCost = edge =>
                        {
                            var result = new PriorityCost(2);
                            float basicEdgeCost = ComputeEdgeCost(edge, DestroyedObstacles);
                            result.AddCost(0, ComputeVisibleTime(edge));
                            result.AddCost(1, basicEdgeCost);
                            return result;
                        };
                    }
                    else
                    {
                        computeCost = edge =>
                        {
                            var result = new PriorityCost(2);
                            float basicEdgeCost = ComputeEdgeCost(edge, DestroyedObstacles);
                            result.AddCost(0, basicEdgeCost);
                            result.AddCost(1, ComputeVisibleTime(edge));
                            return result;
                        };
                    }


                    Func<TileEdge, bool> basicEdgeFilter = Filters.EdgeFilter(UnlockedEdges, character.Data.ForbiddenEdgeTypes,
                        DestroyedDetectors.OfType<BaseEntity>());
                    Func<TileEdge, Dictionary<TileNode, Tuple<TileNode, TileEdge>>, bool> edgeFilter;
                    Path<TileNode, TileEdge> pathToGoal;

                    if (useVisibilityLimit)
                    {
                        
                        pathToGoal = AStarAlgorithm.AStarMultipleVisit(
                            TileNode,
                            GoalNode.TileNode,
                            heuristics,
                            maxAbsoluteVisibility,
                            edgeFilter: basicEdgeFilter,
                            computeCost: computeCost);
                    }
                    else
                    {
                        edgeFilter = (edge, _) => basicEdgeFilter(edge);

                        pathToGoal = AStarAlgorithm.AStar(
                            TileNode,
                            GoalNode.TileNode,
                            heuristics,
                            //Filters.DetectableFilter(DestroyedDetectors, UnlockedTileNodes),
                            edgeFilter: edgeFilter,
                            computeCost: computeCost,
                            costLenght: 2,
                            //onBeforeAddToOpen: edge =>
                            //{
                            //    edge.Neighbor.VisibleTime =
                            //        edge.Start.VisibleTime +
                            //        ComputeVisibleTime(edge);
                            //    edge.Neighbor.TotalTime = edge.Start.TotalTime + edge.Cost;
                            //}
                            secondaryClosedCap: !UsePriorityCost
                        );
                    }

                    TileNode.VisibleTime = VisibleTime;
                    TileNode.TotalTime = TotalTime;
                    GameObject[,] tiles = character.Map.Tiles;


                    if (pathToGoal.Cost < float.MaxValue)
                    {
                        PlanningEdge edge = new PlanningEdge(this, GoalNode, PlanningEdgeType.MONEY, character, pathToGoal, 0, FiniteObject);
                        edges.Add(edge);
                    }

                //    foreach (KeyValuePair<IPlanningEdgeCreator, List<TileNode>> keyValuePair in CreatorsDictionary)
                //    {
                //        IPlanningEdgeCreator creator = keyValuePair.Key;
                //        if (creator.ShouldExplore(this))
                //        {
                //            //Func<TileNode, bool> detectableFilter = Filters.DetectableFilter(DestroyedDetectors, UnlockedTileNodes);
                //            List<TileNode> tileNodes = keyValuePair.Value;

                //            TileNode neighborTileNode = null;
                //            float minDistanceSquared = float.MaxValue;
                //            foreach (TileNode tileNode in tileNodes)
                //            {
                //                if (
                //                    //detectableFilter(tileNode) ||
                //                    tileNode.IsObstructed(DestroyedDetectors.OfType<BaseEntity>())) continue;
                //                var currentPosition = tileNode.Position;
                //                float currentDistanceSquared = Mathf.Pow(currentPosition.First - Position.First, 2) +
                //                                               Mathf.Pow(currentPosition.Second - Position.Second, 2);
                //                if (currentDistanceSquared < minDistanceSquared)
                //                {
                //                    minDistanceSquared = currentDistanceSquared;
                //                    neighborTileNode = tileNode;
                //                }
                //            }

                //            if (null == neighborTileNode)
                //            {
                //                continue;
                //            }

                //            TileNode.VisibleTime = VisibleTime;
                //            TileNode.TotalTime = TotalTime;
                //            Path<TileNode, TileEdge> path = AStarAlgorithm.AStar(
                //                TileNode,
                //                neighborTileNode,
                //                heuristics,
                //                edgeFilter: edgeFilter,
                //                computeCost: computeCost,
                //                onBeforeAddToOpen: edge =>
                //                {
                //                    edge.Neighbor.VisibleTime =
                //                        edge.Start.VisibleTime +
                //                        ComputeVisibleTime(edge);
                //                    edge.Neighbor.TotalTime = edge.Start.TotalTime + edge.Cost;
                //                });

                //            if (path.Edges != null)
                //            {
                //                Dictionary<IPlanningEdgeCreator, List<TileNode>> creatorsCopy =
                //                    CreatorsDictionary.Copy();
                //                creatorsCopy.Remove(creator);

                //                PlanningNode neighbor = new PlanningNode(
                //                    neighborTileNode,
                //                    GoalNode,
                //                    UnlockedEdges.Copy(),
                //                    creatorsCopy,
                //                    character,
                //                    DestroyedObstacles.Copy(),
                //                    DestroyedDetectors.Copy(),
                //                    FiniteObject,
                //                    usePriorityCost: UsePriorityCost);
                //                creator.ModifyNextNode(neighbor);
                //                foreach (TileEdge pathEdge in path.Edges)
                //                {
                //                    IObstacle destroyedObstacle = pathEdge.Obstacle;
                //                    if (destroyedObstacle != null)
                //                    {
                //                        neighbor.DestroyedObstacles.Add(destroyedObstacle);
                //                    }
                //                }

                //                PlanningEdge planningEdge = new PlanningEdge(
                //                    this, 
                //                    neighbor,
                //                    creator.PlanningEdgeType,
                //                    character, 
                //                    path,
                //                    creator.InteractTime,
                //                    creator.Interactable);
                //                edges.Add(planningEdge);
                //            }
                //        }
                //    }
                }
                return edges;
            }
        }

        public float ComputeVisibleTime(TileEdge edge)
        {
            float visibleFraction = edge.Nodes.Count(node => node.IsDetectable(DestroyedDetectors)) /
                                    (float)edge.Nodes.Length;
            return visibleFraction * ComputeEdgeCost(edge, DestroyedObstacles);
        }

        private float ComputeEdgeCost(TileEdge edge, IEnumerable<IObstacle> destroyedObstacles)
        {
            float result = edge.Cost;
            IObstacle obstacle = edge.Obstacle;
            if (obstacle != null && !destroyedObstacles.Contains(obstacle))
            {
                result += obstacle.DelayTime;
            }
            return result;
        }

        public PlanningNode(TileNode tileNode, PlanningNode goalNode, List<EdgeType> unlockedEdges,
            Dictionary<IPlanningEdgeCreator, List<TileNode>> creatorsDictionary, BaseCharacter character,
            List<IObstacle> destroyedObstacles = null, List<DetectorEntity> destroyedDetectors = null,
            GameObject finiteObject = null, IEnumerable<TileNode> unlockedTileNodes = null,
            bool usePriorityCost = false)
        {
            TileNode = tileNode;
            this.GoalNode = goalNode;
            this.CreatorsDictionary = creatorsDictionary;
            this.character = character;
            Position = tileNode.Position;
            UnlockedEdges = unlockedEdges ?? new List<EdgeType>();
            DestroyedObstacles = destroyedObstacles ?? new List<IObstacle>();
            DestroyedDetectors = destroyedDetectors ?? new List<DetectorEntity>();
            this.FiniteObject = finiteObject;
            UsePriorityCost = usePriorityCost;
        }

        public override string ToString()
        {
            var result = new StringBuilder(Position.ToString());
            result.Append(Position);
            foreach (DetectorEntity detector in DestroyedDetectors)
            {
                result.Append(", ");
                result.Append(detector.ToString());
            }
            foreach (IObstacle obstacle in DestroyedObstacles)
            {
                result.Append(", ");
                result.Append(obstacle.InteractableObject);
            }
            foreach (EdgeType edge in UnlockedEdges)
            {
                result.Append(", ");
                result.Append(edge);
            }
            return result.ToString();
        }

        public void Reset()
        {
            edges = null;
        }

        public void UseVisibilityLimit(float maxVisibility)
        {
            useVisibilityLimit = true;
            maxAbsoluteVisibility = maxVisibility;
        }
    }
}
