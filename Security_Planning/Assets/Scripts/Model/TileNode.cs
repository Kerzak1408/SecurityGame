using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using UnityEngine;

public class TileNode : IAStarNode<TileNode>
{
    public IntegerTuple Position;
    public List<Edge> Edges { get; private set; }

    List<IAStarEdge<TileNode>> IAStarNode<TileNode>.Edges
    {
        get { return Edges.OfType<IAStarEdge<TileNode>>().ToList(); }
    }

    public TileNode(int i, int j)
    {
        Position = new IntegerTuple(i, j);
        Edges = new List<Edge>();
    }

    public void AddNeighbor(TileNode node, EdgeType type, float cost)
    {
        Edges.Add(new Edge(node, type, cost));
    }

    public void AddNeighbor(Edge edge)
    {
        Edges.Add(edge);
    }

    public bool HasDirectTransitionTo(IntegerTuple integerTuple)
    {
        return Edges.Any(edge => edge.Type.Equals(EdgeType.NORMAL) && edge.Neighbor.Position.Equals(integerTuple));
    }

    public override string ToString()
    {
        string result = Position.ToString() + ", {";
        foreach (Edge edge in Edges)
        {
            result += edge.Neighbor.Position + ", ";
        }
        result += "}";
        return result;
    }

    public void RemoveAllEdgesBothDirections()
    {
        foreach (Edge edge in Edges)
        {
            List<Edge> neighborEdges = edge.Neighbor.Edges;
            Edge oppositeDirectionEdge = neighborEdges.FirstOrDefault(e => e.Neighbor == this);
            if (!oppositeDirectionEdge.Equals(default(Edge)))
            {
                neighborEdges.Remove(oppositeDirectionEdge);
            }
        }
        Edges.Clear();
    }
}
