﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Model
{
    /// <summary>
    /// Navigation graph node.
    /// </summary>
    public class TileNode : IAStarNode<TileNode>
    {
        public IntegerTuple Position { get; set; }
        public List<TileEdge> Edges { get; private set; }
        public List<DetectorEntity> DetectedBy { get; private set; }
        public Vector3 WorldPosition { get; private set; }

        List<IAStarEdge<TileNode>> IAStarNode<TileNode>.Edges
        {
            get { return Edges.OfType<IAStarEdge<TileNode>>().ToList(); }
        }

        public TileNode(int i, int j, Vector3 worldPosition)
        {
            Position = new IntegerTuple(i, j);
            Edges = new List<TileEdge>();
            DetectedBy = new List<DetectorEntity>();
            WorldPosition = worldPosition;
        }
    
        public void AddNeighbor(TileNode node, EdgeType type, float cost)
        {
            Edges.Add(new TileEdge(this, node, type, cost, null));
        }

        public void AddNeighbor(TileEdge edge)
        {
            Edges.Add(edge);
        }

        public void AddDetector(DetectorEntity detector)
        {
            DetectedBy.Add(detector);
        }

        public bool HasDirectTransitionTo(IntegerTuple integerTuple)
        {
            return Edges.Any(edge => edge.Type.Equals(EdgeType.NORMAL) && edge.Neighbor.Position.Equals(integerTuple));
        }

        public bool IsDetectable(IEnumerable<DetectorEntity> deactivatedDetectors=null)
        {
            IEnumerable<DetectorEntity> filteredDetectors = DetectedBy;
            if (deactivatedDetectors != null)
            {
                return filteredDetectors.Any(detector => !deactivatedDetectors.Contains(detector));
            }
            return filteredDetectors.Count() != 0;
        }

        public override string ToString()
        {
            return "TileNode " + Position.ToString();
        }

        public void RemoveAllEdgesBothDirections()
        {
            foreach (TileEdge edge in Edges)
            {
                List<TileEdge> neighborEdges = edge.Neighbor.Edges;
                TileEdge oppositeDirectionEdge = neighborEdges.FirstOrDefault(e => e.Neighbor == this);
                if (oppositeDirectionEdge != default(TileEdge))
                {
                    neighborEdges.Remove(oppositeDirectionEdge);
                }
            }
            Edges.Clear();
        }

        public bool IsObstructed(IEnumerable<BaseEntity> destroyedObstacles)
        {
            return Edges.All(edge => edge.IsObstructed(destroyedObstacles));
        }
    }
}
