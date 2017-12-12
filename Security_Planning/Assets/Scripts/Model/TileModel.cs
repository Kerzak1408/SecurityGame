using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using UnityEngine;

public class TileModel
{
    public IntegerTuple Position;
    public List<Edge> Neighbors;

    public TileModel(int i, int j)
    {
        Position = new IntegerTuple(i, j);
        Neighbors = new List<Edge>();
    }

    public void AddNeighbor(int i, int j, EdgeType type, float cost)
    {
        Neighbors.Add(new Edge(i, j, type, cost));
    }

    public void AddNeighbor(IntegerTuple indices, EdgeType type, float cost)
    {
        Neighbors.Add(new Edge(indices.First, indices.Second, type, cost));
    }

    public void AddNeighbor(Edge edge)
    {
        Neighbors.Add(edge);
    }

    public bool HasDirectTransitionTo(IntegerTuple integerTuple)
    {
        return Neighbors.Any(neighbor => neighbor.OtherIndices.Equals(integerTuple));
    }
}
