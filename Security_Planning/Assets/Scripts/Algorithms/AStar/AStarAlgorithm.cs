using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Algorithms.AStar.Heuristics;
using Assets.Scripts.Algorithms.AStar.Interfaces;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Extensions;
using Assets.Scripts.Helpers;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Algorithms.AStar
{
    public static class AStarAlgorithm
    {
        /// <summary>
        /// Performs A* search with the given parameters.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="goalNode"></param>
        /// <param name="heuristics">Has to be admissible.</param>
        /// <param name="nodeFilter">A* will NOT exploit any node, for which this filter return TRUE. </param>
        /// <param name="edgeFilter">A* will NOT exploit any edge, for which this filter return TRUE. </param>
        /// <param name="computeCost">This function overrides edge.Cost.</param>
        /// <param name="onBeforeChangeFValue">This function will be called everytime when the F value of the edge.Neighbor should be changed.</param>
        /// <param name="visibilityIndex">Determines the position of the visibility value in PriorityCost.</param>
        /// <returns></returns>
        public static Path<TNode, TEdge> AStar<TNode, TEdge>(TNode startNode, TNode goalNode,
            Heuristics<TNode> heuristics, Func<TNode, bool> nodeFilter = null,
            Func<TEdge, bool> edgeFilter = null, Func<TEdge, PriorityCost> computeCost = null,
            Action<TEdge> onBeforeChangeFValue = null, int visibilityIndex = 1)
            where TNode : IAStarNode<TNode>
            where TEdge : IAStarEdge<TNode>
        {
            bool isPlanning = startNode is PlanningNode;

            if (!isPlanning)
            {
                BenchmarkAStar.NavigationAstars++;
            }

            var closedSet = new List<TNode>();
            var openSet = new List<TNode>
            {
                startNode 
            };

            // Backpointers. Key - where I came, Value-First - from where I came, Value-Second - which edge I used.
            var cameFrom = new Dictionary<TNode, Tuple<TNode, TEdge>>();
        
            var gScores = new LazyDictionary<TNode, PriorityCost>(new PriorityCost(2, float.MaxValue));
            gScores[startNode] = new PriorityCost(2, 0);

            var fScores = new LazyDictionary<TNode, PriorityCost>(new PriorityCost(2, float.MaxValue));
            fScores[startNode] = heuristics.ComputeHeuristics(startNode, goalNode, 2);

            while (openSet.Count != 0)
            {
                if (isPlanning)
                {
                    BenchmarkAStar.PlanningNodesExploited++;
                }
                else
                {
                    BenchmarkAStar.NavigationNodesExploited++;
                }

                TNode currentNode = openSet.First();
            
                // This could be optimized using binary heap but it is not worth it for the small graphs we are using.
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
                
                // Current node is now the one with the smallest F value.
                if (currentNode.Equals(goalNode))
                {
                    var result = ReconstructPath(cameFrom, goalNode, gScores[goalNode][visibilityIndex]);
                    result.Cost = gScores[goalNode][(visibilityIndex + 1) % 2];
                    return result;
                }
            
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (TEdge edge in currentNode.Edges)
                {
                    if (isPlanning)
                    {
                        BenchmarkAStar.PlanningEdgesAccesed++;
                    }
                    else
                    {
                        BenchmarkAStar.NavigationEdgesAccessed++;
                    }

                    TNode neighbor = edge.Neighbor;

                    if (nodeFilter != null && nodeFilter(neighbor) || 
                        edgeFilter != null && edgeFilter(edge))
                    {
                        continue;
                    }

                    PriorityCost cost = computeCost == null ? edge.Cost : computeCost(edge);
                    PriorityCost potentialGScore = gScores[currentNode] + cost;
                    //potentialGScore.Round(2);

                    if (closedSet.Contains(neighbor)) continue;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }

                    // Check if we can improve scores.
                    if (potentialGScore < gScores[neighbor])
                    {
                        if (onBeforeChangeFValue != null)
                        {
                            onBeforeChangeFValue(edge);
                        }
                        cameFrom[neighbor] = new Tuple<TNode, TEdge>(currentNode, edge); ;
                        gScores[neighbor] = potentialGScore;
                        var hScore = heuristics.ComputeHeuristics(neighbor, goalNode, 2);
                        //hScore.Round(2);
                        fScores[neighbor] = potentialGScore + hScore;
                    }
                }
            }
            // Path not found.
            return new Path<TNode, TEdge>(null, float.MaxValue);
        }

        /// <summary>
        /// Similar to A* but plans within the space of tuples (node, visibility_so_far).
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="heuristics">Has to be admissible.</param>
        /// <param name="maxVisibility">Visibility limit, paths that exceed this limit will NOT be considered.</param>
        /// <param name="nodeFilter">A* will NOT exploit any node, for which this filter return TRUE. </param>
        /// <param name="edgeFilter">A* will NOT exploit any edge, for which this filter return TRUE. </param>
        /// <param name="computeCost">This function overrides edge.Cost.</param>
        /// <returns></returns>
        public static Path<TNode, TEdge> AStarMultipleVisit<TNode, TEdge>(TNode startNode, TNode endNode,
            Heuristics<TNode> heuristics, float maxVisibility, Func<TNode, bool> nodeFilter = null,
            Func<TEdge, bool> edgeFilter = null, Func<TEdge, PriorityCost> computeCost = null)
            where TNode : IAStarNode<TNode>
            where TEdge : IAStarEdge<TNode>
        {
            bool isPlanning = startNode is PlanningNode;
            if (!isPlanning)
            {
                BenchmarkAStar.NavigationAstarsMultiple++;
            }

            var closedSet = new List<Tuple<TNode, float>>();
            var openSet = new List<Tuple<TNode, float>>();
            var startTuple = new Tuple<TNode, float>(startNode, 0);
            openSet.Add(startTuple);

            // Backpointers. Key - where I came, Value-First - from where I came, Value-Second - which edge I used.
            var cameFrom = new Dictionary<Tuple<TNode, float>, Tuple<Tuple<TNode, float>, TEdge>>();

            var gScores = new LazyDictionary<Tuple<TNode, float>, PriorityCost>(new PriorityCost(2, float.MaxValue));
            gScores[startTuple] = new PriorityCost(2, 0);

            var fScores = new LazyDictionary<Tuple<TNode, float>, PriorityCost>(new PriorityCost(2, float.MaxValue));
            fScores[startTuple] = heuristics.ComputeHeuristics(startNode, endNode, 2);


            while (openSet.Count != 0)
            {
                if (isPlanning)
                {
                    BenchmarkAStar.PlanningNodesExploited++;
                }
                else
                {
                    BenchmarkAStar.NavigationNodesExploited++;
                }
                Tuple<TNode, float> currentTuple = openSet.First();
                PriorityCost minFValue = fScores[currentTuple];
                foreach (Tuple<TNode, float> tuple in openSet)
                {
                    var fValue = fScores[tuple];
                    if (fValue[0] < minFValue[0])
                    {
                        currentTuple = tuple;
                        minFValue = fValue;
                    }
                }

                if (currentTuple.First.Equals(endNode))
                {
                    var result =  ReconstructPath(cameFrom, currentTuple, currentTuple.Second);
                    result.Cost = gScores[currentTuple][0];
                    return result;
                }
            
                openSet.Remove(currentTuple);
                closedSet.Add(currentTuple);

                foreach (TEdge edge in currentTuple.First.Edges)
                {
                    if (isPlanning)
                    {
                        BenchmarkAStar.PlanningEdgesAccesed++;
                    }
                    else
                    {
                        BenchmarkAStar.NavigationEdgesAccessed++;
                    }

                    TNode neighbor = edge.Neighbor;

                    if (nodeFilter != null && nodeFilter(neighbor) ||
                        edgeFilter != null && edgeFilter(edge))
                    {
                        continue;
                    }
                

                    PriorityCost cost = computeCost == null ? edge.Cost : computeCost(edge);
                    PriorityCost potentialGScore = gScores[currentTuple] + cost;

                    var neighborTuple = new Tuple<TNode, float>(neighbor, potentialGScore[1]);

                    // Do NOT exploit the edge if its adding would cause exceeding the maximal visibility.
                    if (potentialGScore[1] > maxVisibility) continue;
                    if (closedSet.Contains(neighborTuple)) continue;

                    if (!openSet.Contains(neighborTuple))
                    {
                        // Here comes the tricky part. We have to check not only the exploited neighbor but all the tuples in closed and open set that
                        // represents the same node as neighbor.
                        IEnumerable<Tuple<TNode, float>> neighborOpenSetOccurences = openSet.Where(tuple => tuple.First.Equals(neighbor)).ToList().Copy();
                    
                        // We want to add the exploited tuple into the open set if it is better in visibility or path length in comparison with
                        // every other tuple in both closed and open set.
                        bool shoudAddToOpen = true;
                        foreach (Tuple<TNode, float> neighborOccurence in neighborOpenSetOccurences)
                        {
                            PriorityCost neighborGScore = gScores[neighborOccurence];
                            float neighborLength = neighborGScore[0];
                            float neighborVisibility = neighborGScore[1];
                            float potentialLength = potentialGScore[0];
                            float potentialVisibility = potentialGScore[1];
                            float lengthDiff = potentialLength - neighborLength;
                            float visibilityDiff = potentialVisibility - neighborVisibility;
                            
                            if (lengthDiff < -1e-4 || visibilityDiff < -1e-4)
                            {
                                if ((lengthDiff < -1e-4 && visibilityDiff < 1e-4) ||
                                    (lengthDiff < 1e-4 && visibilityDiff < -1e-4))
                                {
                                    // If both parameters (visibility and path length) of the exploited occurence are better than the ones
                                    // in the open set, remove the worse one configuration from the open set.
                                    openSet.Remove(neighborOccurence);
                                    fScores.Remove(neighborOccurence);
                                    gScores.Remove(neighborOccurence);
                                }
                            }
                            else
                            {
                                // We found an occurence of a neighbor node in the open set that has lower/equal both visibility and path length.
                                // Therefore, it does not make sense to exploit this tuple. This is the crucial time saving check.
                                shoudAddToOpen = false;
                            }
                        }

                        IEnumerable<Tuple<TNode, float>> neighborClosedSetOccurences = closedSet.Where(tuple => tuple.First.Equals(neighbor)).ToList().Copy();
                        foreach (Tuple<TNode, float> neighborOccurence in neighborClosedSetOccurences)
                        {
                            PriorityCost neighborGScore = gScores[neighborOccurence];
                            float neighborLength = neighborGScore[0];
                            float neighborVisibility = neighborGScore[1];
                            float potentialLength = potentialGScore[0];
                            float potentialVisibility = potentialGScore[1];
                            float lengthDiff = potentialLength - neighborLength;
                            float visibilityDiff = potentialVisibility - neighborVisibility;
                            if (lengthDiff > -1e-4 && visibilityDiff > -1e-4)
                            {
                                // Same as in the open set. Better node configuration is already closed -> reason to exploit this one.
                                shoudAddToOpen = false;
                            }
                        }

                        if (shoudAddToOpen)
                        {
                            openSet.Add(neighborTuple);
                        }
                    }


                    if (openSet.Contains(neighborTuple) && potentialGScore < gScores[neighborTuple])
                    {
                        cameFrom[neighborTuple] = new Tuple<Tuple<TNode, float>, TEdge>(currentTuple, edge);
                        gScores[neighborTuple] = potentialGScore;
                        PriorityCost fScore = potentialGScore + heuristics.ComputeHeuristics(neighbor, endNode, 2);
                        fScores[neighborTuple] = fScore;
                    }
                }
            }
            return new Path<TNode, TEdge>(null, float.MaxValue);
        }

        // Reconstruct path using dictionary of backpointers.
        private static Path<TNode, TEdge> ReconstructPath<TNode, TEdge>(Dictionary<TNode, Tuple<TNode, TEdge>> pathDictionary, TNode endNode, float visibility)
            where TNode : IAStarNode<TNode>
            where TEdge : IAStarEdge<TNode>
        {
            var result = new Path<TNode, TEdge>{VisibilityTime = visibility};
            TNode current = endNode;
            while (pathDictionary.Keys.Contains(current))
            {
                Tuple<TNode, TEdge> currentTuple = pathDictionary[current];
                result.AddEdgeToBeginning(currentTuple.Second);
                current = currentTuple.First;
            }
            return result;
        }

        // Reconstruct path using dictionary of backpointers.
        private static Path<TNode, TEdge> ReconstructPath<TNode, TEdge>(Dictionary<Tuple<TNode, float>, Tuple<Tuple<TNode, float>, TEdge>> pathDictionary, 
            Tuple<TNode, float> endNode, float visibility)
            where TNode : IAStarNode<TNode>
            where TEdge : IAStarEdge<TNode>
        {
            var result = new Path<TNode, TEdge> {VisibilityTime = visibility};
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
}
