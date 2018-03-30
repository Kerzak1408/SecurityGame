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

        private PlanningNode goalNode;
        private Dictionary<IPlanningEdgeCreator, List<TileNode>> creatorsDictionary;
        private BaseCharacter character;
        private List<IAStarEdge<PlanningNode>> edges;
        private GameObject finiteObject;

        public List<IAStarEdge<PlanningNode>> Edges
        {
            get
            {
                if (edges == null)
                {
                    edges = new List<IAStarEdge<PlanningNode>>();
                    GameObject[,] tiles = character.CurrentGame.Map.Tiles;
                    Path<TileNode, TileEdge> pathToGoal = AStarAlgorithm.AStar(TileNode, goalNode.TileNode,
                            new EuclideanHeuristics<TileNode>(tiles),
                            Filters.DetectableFilter(DestroyedDetectors),
                            Filters.EdgeFilter(UnlockedEdges, character.Data.ForbiddenEdgeTypes),
                            edge => ComputeEdgeCost(edge, DestroyedObstacles));

                    if (pathToGoal.Cost < float.MaxValue)
                    {
                        PlanningEdge edge = new PlanningEdge(this, goalNode, PlanningEdgeType.MONEY, character, pathToGoal, finiteObject);
                        edges.Add(edge);
                    }
                    foreach (KeyValuePair<IPlanningEdgeCreator, List<TileNode>> keyValuePair in creatorsDictionary)
                    {
                        IPlanningEdgeCreator creator = keyValuePair.Key;
                        if (creator.ShouldExplore(this))
                        {
                            Func<TileNode, bool> detectableFilter = Filters.DetectableFilter(DestroyedDetectors);
                            List<TileNode> tileNodes = keyValuePair.Value;
                            //IEnumerable<TileNode> availableTileNodes = tileNodes.Where(node => !detectableFilter(node));
                            //float closestDistance = availableTileNodes.Min(tileNode => Vector3.Distance(tileNode.Position, Position));

                            //TileNode neighborTileNode = availableTileNodes.FirstOrDefault(tileNode =>
                            //    Vector3.Distance(tileNode.Position, Position) == closestDistance);
                            TileNode neighborTileNode = tileNodes.FirstOrDefault(node => !detectableFilter(node));
                            if (null == neighborTileNode)
                            {
                                continue;
                            }
                            Path<TileNode, TileEdge> path = AStarAlgorithm.AStar(TileNode, neighborTileNode,
                                    new EuclideanHeuristics<TileNode>(tiles),
                                    Filters.DetectableFilter(DestroyedDetectors),
                                    Filters.EdgeFilter(UnlockedEdges, character.Data.ForbiddenEdgeTypes),
                                    edge => ComputeEdgeCost(edge, DestroyedObstacles));

                            if (path.Edges != null)
                            {
                                Dictionary<IPlanningEdgeCreator, List<TileNode>> creatorsCopy =
                                    creatorsDictionary.Copy();
                                creatorsCopy.Remove(creator);

                                PlanningNode neighbor = new PlanningNode(
                                    neighborTileNode,
                                    goalNode,
                                    UnlockedEdges.Copy(),
                                    creatorsCopy,
                                    character,
                                    DestroyedObstacles.Copy(),
                                    DestroyedDetectors.Copy(),
                                    finiteObject);
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
                                    creator.GameObject);
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
            GameObject finiteObject = null)
        {
            TileNode = tileNode;
            this.goalNode = goalNode;
            this.creatorsDictionary = creatorsDictionary;
            this.character = character;
            Position = tileNode.Position;
            UnlockedEdges = unlockedEdges;
            DestroyedObstacles = destroyedObstacles ?? new List<IObstacle>();
            DestroyedDetectors = destroyedDetectors ?? new List<DetectorEntity>();
            this.finiteObject = finiteObject;
        }

        public override string ToString()
        {
            var result = new StringBuilder(Position.ToString());
            foreach (DetectorEntity detector in DestroyedDetectors)
            {
                result.Append(", ");
                result.Append(detector.ToString());
            }
            foreach (IObstacle obstacle in DestroyedObstacles)
            {
                result.Append(", ");
                result.Append(obstacle.InteractableObject.ToString());
            }
            foreach (EdgeType edge in UnlockedEdges)
            {
                result.Append(", ");
                result.Append(edge.ToString());
            }
            return result.ToString();
        }
    }
}
