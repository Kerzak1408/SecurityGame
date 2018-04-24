using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Algorithms.AStar;
using Assets.Scripts.Algorithms.AStar.Heuristics;
using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
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
        private readonly BaseCharacter character;
        private List<IAStarEdge<PlanningNode>> edges;
        private bool useVisibilityLimit;
        private float maxAbsoluteVisibility;
        private float lowVisibilityLimit;
        private float highVisibilityLimit;

        public GameObject FiniteObject { get; set; }
        public bool IsVisibilityPriority { get; set; }
        public float VisibleTime { get; set; }

        private Func<TileEdge, PriorityCost> CurrentPriorityCost
        {
            get
            {
                return edge =>
                {
                    var result = new PriorityCost(2);
                    float basicEdgeCost = ComputeEdgeCost(edge, DestroyedObstacles);
                    result.AddCost(IsVisibilityPriority ? 0 : 1, ComputeVisibleTime(edge));
                    result.AddCost(IsVisibilityPriority ? 1 : 0, basicEdgeCost);
                    return result;
                };
            }
        }

        /// <summary>
        /// Edges are generated dynamically so we do not have to create fully connected graph.
        /// </summary>
        public List<IAStarEdge<PlanningNode>> Edges
        {
            get
            {
                if (edges == null)
                {
                    edges = new List<IAStarEdge<PlanningNode>>();
                    // Try to find path to the goal.
                    Path<TileNode, TileEdge> pathToGoal = ComputePath(GoalNode.TileNode, maxAbsoluteVisibility - VisibleTime);

                    if (pathToGoal.Cost < float.MaxValue)
                    {
                        PlanningEdge edge = new PlanningEdge(this, GoalNode, PlanningEdgeType.MONEY, character, pathToGoal, 0, FiniteObject);
                        edges.Add(edge);
                    }

                    // Look for all available not-yet-visited interactable items.
                    foreach (KeyValuePair<IPlanningEdgeCreator, List<TileNode>> keyValuePair in CreatorsDictionary)
                    {
                        IPlanningEdgeCreator creator = keyValuePair.Key;
                        if (creator.ShouldExplore(this))
                        {
                            IEnumerable<TileNode> tileNodes = keyValuePair.Value;

                            // Each interactable item can be surrounded by many navigation nodes. We choose just the closest one 
                            // from this set to reduce the branching of the search.
                            TileNode neighborTileNode = null;
                            float minDistanceSquared = float.MaxValue;
                            foreach (TileNode tileNode in tileNodes)
                            {
                                if (tileNode.IsObstructed(DestroyedDetectors.OfType<BaseEntity>())) continue;
                                var currentPosition = tileNode.Position;
                                float currentDistanceSquared = Mathf.Pow(currentPosition.First - Position.First, 2) +
                                                               Mathf.Pow(currentPosition.Second - Position.Second, 2);
                                if (currentDistanceSquared < minDistanceSquared)
                                {
                                    minDistanceSquared = currentDistanceSquared;
                                    neighborTileNode = tileNode;
                                }
                            }

                            if (null == neighborTileNode)
                            {
                                continue;
                            }

                            int branchesMinusOne = character.Data.Sensitivity - 1;
                            for (int i = 0; i <= branchesMinusOne; i++)
                            {
                                float currentVisibilityLimit = branchesMinusOne == 0 ?
                                    0
                                    :
                                    lowVisibilityLimit + (float) i / branchesMinusOne *
                                    (highVisibilityLimit - lowVisibilityLimit);
                                Path<TileNode, TileEdge> path = ComputePath(neighborTileNode, currentVisibilityLimit);
                                float potentialvisibility = VisibleTime + path.VisibilityTime;
                                if (path.Edges != null && (!useVisibilityLimit || potentialvisibility <= maxAbsoluteVisibility))
                                {
                                    Dictionary<IPlanningEdgeCreator, List<TileNode>> creatorsCopy =
                                        CreatorsDictionary.Copy();
                                    creatorsCopy.Remove(creator);

                                    PlanningNode neighbor = new PlanningNode(
                                        neighborTileNode,
                                        GoalNode,
                                        UnlockedEdges.Copy(),
                                        creatorsCopy,
                                        character,
                                        DestroyedObstacles.Copy(),
                                        DestroyedDetectors.Copy(),
                                        FiniteObject,
                                        IsVisibilityPriority);
                                    creator.ModifyNextNode(neighbor);
                                    foreach (TileEdge pathEdge in path.Edges)
                                    {
                                        IObstacle destroyedObstacle = pathEdge.Obstacle;
                                        if (destroyedObstacle != null)
                                        {
                                            neighbor.DestroyedObstacles.Add(destroyedObstacle);
                                        }
                                    }

                                    PlanningEdge planningEdge = new PlanningEdge(
                                        this,
                                        neighbor,
                                        creator.PlanningEdgeType,
                                        character,
                                        path,
                                        creator.InteractTime,
                                        creator.Interactable);
                                    edges.Add(planningEdge);
                                    if (!useVisibilityLimit)
                                    {
                                        break;
                                    }
                                }
                            }

                            
                        }
                    }
                }
                return edges;
            }
        }

        private Path<TileNode, TileEdge> ComputePath(TileNode target, float maxVisibility)
        {
            Func<TileEdge, bool> basicEdgeFilter = Filters.EdgeFilter(UnlockedEdges, character.Data.ForbiddenEdgeTypes,
                        DestroyedDetectors.OfType<BaseEntity>());

            Heuristics<TileNode> heuristics = new EuclideanHeuristics<TileNode>(IsVisibilityPriority ? 1 : 0);
            if (useVisibilityLimit)
            {
                return AStarAlgorithm.AStarMultipleVisit(
                    TileNode,
                    target,
                    heuristics,
                    maxVisibility,
                    edgeFilter: basicEdgeFilter,
                    computeCost: CurrentPriorityCost);
            }
            // Simple A* in case there is no visibility limit.
            return AStarAlgorithm.AStar(
                TileNode,
                target,
                heuristics,
                edgeFilter: basicEdgeFilter,
                computeCost: CurrentPriorityCost,
                visibilityIndex : IsVisibilityPriority ? 0 : 1
            );
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
            GameObject finiteObject = null,
            bool isVisibilityPriority = false)
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
            IsVisibilityPriority = isVisibilityPriority;
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

        /// <summary>
        /// Clears edges.
        /// </summary>
        public void Reset()
        {
            edges = null;
        }

        public void UseVisibilityLimit(float maxVisibility, float lowVisibilityLimit, float highVisibilityLimit)
        {
            useVisibilityLimit = true;
            maxAbsoluteVisibility = maxVisibility;
            this.lowVisibilityLimit = lowVisibilityLimit;
            this.highVisibilityLimit = highVisibilityLimit;
        }

        public PlanningNode Copy()
        {
            return new PlanningNode(TileNode, GoalNode, UnlockedEdges, CreatorsDictionary, character,
                DestroyedObstacles, DestroyedDetectors, FiniteObject, IsVisibilityPriority);
        }
    }
}
