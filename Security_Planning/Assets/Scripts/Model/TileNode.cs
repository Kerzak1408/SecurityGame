using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Model;
using UnityEngine;

public class TileNode : IAStarNode<TileNode>
{
    public IntegerTuple Position { get; set; }
    public List<TileEdge> Edges { get; private set; }
    public List<DetectorEntity> DetectedBy { get; private set; }

    List<IAStarEdge<TileNode>> IAStarNode<TileNode>.Edges
    {
        get { return Edges.OfType<IAStarEdge<TileNode>>().ToList(); }
    }

    public TileNode(int i, int j)
    {
        Position = new IntegerTuple(i, j);
        Edges = new List<TileEdge>();
        DetectedBy = new List<DetectorEntity>();
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

    public bool IsDetectable(IEnumerable<DetectorEntity> deactivatedDetectors=null, IEnumerable<DetectorType> ignoredTypes=null)
    {
        IEnumerable<DetectorEntity> filteredDetectors = DetectedBy;
        if (ignoredTypes != null)
        {
            filteredDetectors =
                filteredDetectors.Where(detector => !ignoredTypes.Contains(detector.DetectorType));
        }
        if (deactivatedDetectors != null)
        {
            return filteredDetectors.Any(detector => !deactivatedDetectors.Contains(detector));
        }
        return filteredDetectors.Count() != 0;
    }

    public override string ToString()
    {
        string result = Position.ToString() + ", {";
        foreach (TileEdge edge in Edges)
        {
            result += edge.Neighbor.Position + ", ";
        }
        result += "}";
        return result;
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
}
