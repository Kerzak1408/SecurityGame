using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;
using System;
using Assets.Scripts.Algorithms.FloodFill;

public static class FloodFillAlgorithm
{
    public static TCluster FloodFill<TCluster, TNode>(TNode startNode, Func<TNode, IEnumerable<TNode>> getNeighbors) where TCluster : ICluster<TNode>, new()
    {
        var result = new TCluster();
        FloodFill(startNode, getNeighbors, new List<TNode>(), result);
        return result;
    }

    public static TCluster FloodFill<TCluster, TNode>(TNode currentNode, Func<TNode, IEnumerable<TNode>> getNeighbors,
        List<TNode> visited, TCluster result) 
        where TCluster : ICluster<TNode>, new()
    {
        visited.Add(currentNode);
        result.Members.Add(currentNode);
        foreach (TNode neighbor in getNeighbors(currentNode))
        {
            if (!visited.Contains(neighbor))
            {
                FloodFill(neighbor, getNeighbors, visited, result);
            }
        }
        return result;
    }

    public static List<TCluster> GenerateClusters<TCluster, TNode>(IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>> getNeighbors) 
        where TCluster : ICluster<TNode>, new()
    {
        var result = new List<TCluster>();
        var alreadyClustered = new List<TNode>();
        foreach (TNode node in nodes)
        {
            if (alreadyClustered.Contains(node))
            {
                continue;
            }

            TCluster cluster = FloodFill<TCluster, TNode>(node, getNeighbors);
            result.Add(cluster);
            foreach (TNode clusterMember in cluster.Members)
            {
                alreadyClustered.Add(clusterMember);
            }
        }
        return result;
    }
}
