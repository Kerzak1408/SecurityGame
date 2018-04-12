using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public IEnumerable<TileNode> UnlockedTileNodes { get; set; }

        public Dictionary<IPlanningEdgeCreator, List<TileNode>> CreatorsDictionary { get; set; }
        private BaseCharacter character;
        private List<IAStarEdge<PlanningNode>> edges;

        public GameObject FiniteObject { get; set; }

        public List<IAStarEdge<PlanningNode>> Edges
        {
            get
            {
                if (edges == null)
                {
                    edges = new List<IAStarEdge<PlanningNode>>();
                    GameObject[,] tiles = character.Map.Tiles;
                    Path<TileNode, TileEdge> pathToGoal = AStarAlgorithm.AStar(TileNode, GoalNode.TileNode,
                            new EuclideanHeuristics<TileNode>(tiles),
                            Filters.DetectableFilter(DestroyedDetectors, UnlockedTileNodes),
                            Filters.EdgeFilter(UnlockedEdges, character.Data.ForbiddenEdgeTypes, DestroyedDetectors.OfType<BaseEntity>()),
                            edge => ComputeEdgeCost(edge, DestroyedObstacles));

                    if (pathToGoal.Cost < float.MaxValue)
                    {
                        PlanningEdge edge = new PlanningEdge(this, GoalNode, PlanningEdgeType.MONEY, character, pathToGoal, 0, FiniteObject);
                        edges.Add(edge);
                    }
                    foreach (KeyValuePair<IPlanningEdgeCreator, List<TileNode>> keyValuePair in CreatorsDictionary)
                    {
                        IPlanningEdgeCreator creator = keyValuePair.Key;
                        if (creator.ShouldExplore(this))
                        {
                            Func<TileNode, bool> detectableFilter = Filters.DetectableFilter(DestroyedDetectors, UnlockedTileNodes);
                            List<TileNode> tileNodes = keyValuePair.Value;

                            TileNode neighborTileNode = null;
                            float minDistanceSquared = float.MaxValue;
                            foreach (TileNode tileNode in tileNodes)
                            {
                                if (detectableFilter(tileNode) || tileNode.IsObstructed(DestroyedDetectors.OfType<BaseEntity>())) continue;
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
                            Path<TileNode, TileEdge> path = AStarAlgorithm.AStar(TileNode, neighborTileNode,
                                    new EuclideanHeuristics<TileNode>(tiles),
                                    detectableFilter,
                                    Filters.EdgeFilter(UnlockedEdges, character.Data.ForbiddenEdgeTypes, DestroyedDetectors.OfType<BaseEntity>()),
                                    edge => ComputeEdgeCost(edge, DestroyedObstacles));

                            if (path.Edges != null)
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
                                    UnlockedTileNodes);
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
                            }
                        }
                    }
                }
                return edges;
            }
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
            GameObject finiteObject = null, IEnumerable<TileNode> unlockedTileNodes = null)
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
            UnlockedTileNodes = unlockedTileNodes;
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
    }
}
