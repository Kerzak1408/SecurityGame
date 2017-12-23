using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using UnityEngine;

public class TileNode : IAStarNode
{
    public IntegerTuple Position;
    public List<Edge> Edges { get; private set; }

    List<IAStarEdge> IAStarNode.Edges
    {
        get { return Edges.OfType<IAStarEdge>().ToList(); }
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
}
