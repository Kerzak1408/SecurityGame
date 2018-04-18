using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using UnityEngine;

public static class AStarAlgorithm
{
    static int logindex;

    public static Path<TNode, TEdge> AStar<TNode, TEdge>(TNode startNode, TNode endNode,
        Heuristics<TNode> heuristics, Func<TNode, bool> nodeFilter = null,
        Func<TEdge, Dictionary<TNode, Tuple<TNode, TEdge>>, bool> edgeFilter = null, Func<TEdge, PriorityCost> computeCost = null, int costLenght = 1,
        Action<TEdge> onBeforeAddToOpen = null, bool secondaryClosedCap = false)
        where TNode : IAStarNode<TNode>
        where TEdge : IAStarEdge<TNode>
    {
        var closedSet = new List<TNode>();
        //var closedDictionary = new Dictionary<TNode, float>();
        var openSet = new List<TNode>
        {
            startNode 
        };

        StreamWriter logFileWriter = new StreamWriter(FileHelper.JoinPath(Application.dataPath, "astarLog_" + startNode.GetType() + "_" +
            startNode.Position + "-" + endNode.Position + "_"  + logindex++ + ".txt"));

        // Key - where I came, Value-First - from where I came, Value-Second - which edge I used.
        var cameFrom = new Dictionary<TNode, Tuple<TNode, TEdge>>();
        
        var gScores = new LazyDictionary<TNode, PriorityCost>(new PriorityCost(costLenght, float.MaxValue));
        gScores[startNode] = new PriorityCost(costLenght, 0);

        var fScores = new LazyDictionary<TNode, PriorityCost>(new PriorityCost(costLenght, float.MaxValue));
        fScores[startNode] = heuristics.ComputeHeuristics(startNode, endNode, costLenght);

        while (openSet.Count != 0)
        {
            TNode currentNode = openSet.First();
            //log("AStar, exploiting node: " + currentNode);
            PriorityCost minFValue = fScores[currentNode];
            foreach (TNode node in openSet)
            {
                var fValue = fScores[node];
                if (fValue < minFValue)
                {
                    currentNode = node;
                    minFValue = fValue;
                }
            }            

            if (currentNode.Equals(endNode))
            {
                logFileWriter.Close();
                return ReconstructPath(cameFrom, endNode);
            }

            logFileWriter.WriteLine(" exploiting: " + currentNode);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            //if (secondaryClosedCap)
            //{
            //    closedDictionary[currentNode] = gScores[currentNode][1];
            //}


            foreach (TEdge edge in currentNode.Edges)
            {
                TNode neighbor = edge.Neighbor;

                if (
                    (nodeFilter != null && nodeFilter(neighbor)) || 
                    (edgeFilter != null && edgeFilter(edge, cameFrom)))
                {
                    logFileWriter.WriteLine("Filtered: " + edge + " is in closed: " +
                        closedSet.Contains(neighbor) + " edge filtered : " + (edgeFilter != null && edgeFilter(edge, cameFrom)));
                    continue;
                }

                PriorityCost cost = computeCost == null ? edge.Cost : computeCost(edge);
                PriorityCost potentialGScore = gScores[currentNode] + cost;

                //if (secondaryClosedCap)
                //{
                //    if (closedDictionary.ContainsKey(neighbor) && potentialGScore[1] >= closedDictionary[neighbor])
                //    {
                //        continue;
                //    }
                //}
                //else
                {
                    if (closedSet.Contains(neighbor)) continue;
                }


                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }


                if (potentialGScore < gScores[neighbor]
                    //||
                    //(secondaryClosedCap && closedDictionary.ContainsKey(neighbor) && potentialGScore[1] < closedDictionary[neighbor])
                    )
                {
                    if (onBeforeAddToOpen != null)
                    {
                        onBeforeAddToOpen(edge);
                    }
                    cameFrom[neighbor] = new Tuple<TNode, TEdge>(currentNode, edge); ;
                    gScores[neighbor] = potentialGScore;
                    fScores[neighbor] = potentialGScore + heuristics.ComputeHeuristics(neighbor, endNode, costLenght);
                }
            }
        }
        logFileWriter.Close();
        return new Path<TNode, TEdge>(null, float.MaxValue);
    }

    public static Path<TNode, TEdge> AStarMultipleVisit<TNode, TEdge>(TNode startNode, TNode endNode,
        Heuristics<TNode> heuristics, float maxVisibility, Func<TNode, bool> nodeFilter = null,
        Func<TEdge, bool> edgeFilter = null, Func<TEdge, PriorityCost> computeCost = null)
        where TNode : IAStarNode<TNode>
        where TEdge : IAStarEdge<TNode>
    {
        var closedSet = new List<Tuple<TNode, float>>();
        //var closedDictionary = new Dictionary<TNode, float>();
        var openSet = new List<Tuple<TNode, float>>();
        var startTuple = new Tuple<TNode, float>(startNode, 0);
        openSet.Add(startTuple);

        StreamWriter logFileWriter = new StreamWriter(FileHelper.JoinPath(Application.dataPath, "astarLog_" + startNode.GetType() + "_" +
            startNode.Position + "-" + endNode.Position + "_" + logindex++ + ".txt"));

        // Key - where I came, Value-First - from where I came, Value-Second - which edge I used.
        var cameFrom = new Dictionary<Tuple<TNode, float>, Tuple<Tuple<TNode, float>, TEdge>>();

        var gScores = new LazyDictionary<Tuple<TNode, float>, PriorityCost>(new PriorityCost(2, float.MaxValue));
        gScores[startTuple] = new PriorityCost(2, 0);

        var fScores = new LazyDictionary<Tuple<TNode, float>, PriorityCost>(new PriorityCost(2, float.MaxValue));
        fScores[startTuple] = heuristics.ComputeHeuristics(startNode, endNode, 2);

        while (openSet.Count != 0)
        {
            Tuple<TNode, float> currentTuple = openSet.First();
            //log("AStar, exploiting node: " + currentNode);
            PriorityCost minFValue = fScores[currentTuple];
            foreach (Tuple<TNode, float> tuple in openSet)
            {
                var fValue = fScores[tuple];
                if (fValue < minFValue)
                {
                    currentTuple = tuple;
                    minFValue = fValue;
                }
            }

            if (currentTuple.First.Equals(endNode))
            {
                logFileWriter.Close();
                return ReconstructPath(cameFrom, currentTuple);
            }

            logFileWriter.WriteLine(" exploiting: " + currentTuple);
            openSet.Remove(currentTuple);
            closedSet.Add(currentTuple);
            //if (secondaryClosedCap)
            //{
            //    closedDictionary[currentNode] = gScores[currentNode][1];
            //}


            foreach (TEdge edge in currentTuple.First.Edges)
            {
                TNode neighbor = edge.Neighbor;

                if ((nodeFilter != null && nodeFilter(neighbor)) ||
                    (edgeFilter != null && edgeFilter(edge)))
                {
                    //logFileWriter.WriteLine("Filtered: " + edge + " is in closed: " +
                    //    closedSet.Contains(neighbor) + " edge filtered : " + (edgeFilter != null && edgeFilter(edge, cameFrom)));
                    continue;
                }
                

                PriorityCost cost = computeCost == null ? edge.Cost : computeCost(edge);
                PriorityCost potentialGScore = gScores[currentTuple] + cost;
                var neighborTuple = new Tuple<TNode, float>(neighbor, potentialGScore[1]);

                if (potentialGScore[1] > maxVisibility) continue;
                if (closedSet.Contains(neighborTuple)) continue;

                if (!openSet.Contains(neighborTuple))
                {
                    openSet.Add(neighborTuple);
                }


                if (potentialGScore < gScores[neighborTuple]
                    //||
                    //(secondaryClosedCap && closedDictionary.ContainsKey(neighbor) && potentialGScore[1] < closedDictionary[neighbor])
                    )
                {
                    //if (onBeforeAddToOpen != null)
                    //{
                    //    onBeforeAddToOpen(edge);
                    //}
                    cameFrom[neighborTuple] = new Tuple<Tuple<TNode, float>, TEdge>(currentTuple, edge);
                    gScores[neighborTuple] = potentialGScore;
                    fScores[neighborTuple] = potentialGScore + heuristics.ComputeHeuristics(neighbor, endNode, 2);
                }
            }
        }
        logFileWriter.Close();
        return new Path<TNode, TEdge>(null, float.MaxValue);
    }

    private static Path<TNode, TEdge> ReconstructPath<TNode, TEdge>(Dictionary<TNode, Tuple<TNode, TEdge>> pathDictionary, TNode endNode)
        where TNode : IAStarNode<TNode>
        where TEdge : IAStarEdge<TNode>
    {
        var result = new Path<TNode, TEdge>();
        TNode current = endNode;
        while (pathDictionary.Keys.Contains(current))
        {
            Tuple<TNode, TEdge> currentTuple = pathDictionary[current];
            result.AddEdgeToBeginning(currentTuple.Second);
            current = currentTuple.First;
        }
        return result;
    }

    private static Path<TNode, TEdge> ReconstructPath<TNode, TEdge>(Dictionary<Tuple<TNode, float>, Tuple<Tuple<TNode, float>, TEdge>> pathDictionary, 
        Tuple<TNode, float> endNode)
    where TNode : IAStarNode<TNode>
    where TEdge : IAStarEdge<TNode>
    {
        var result = new Path<TNode, TEdge>();
        Tuple<TNode, float> current = endNode;
        while (pathDictionary.Keys.Contains(current))
        {
            Tuple<Tuple<TNode, float>, TEdge> currentTuple = pathDictionary[current];
            result.AddEdgeToBeginning(currentTuple.Second);
            current = currentTuple.First;
        }
        return result;
    }
}
