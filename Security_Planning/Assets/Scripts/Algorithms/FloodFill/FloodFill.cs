using System;
using System.Collections.Generic;
using Assets.Scripts.Algorithms.FloodFill.Interfaces;

namespace Assets.Scripts.Algorithms.FloodFill
{
    public static class FloodFillAlgorithm
    {
        /// <summary>
        /// Wrapper for the recursive Floodfill. This method just initializes result, visited list and runs recursive FloodFill ->
        /// <see cref="FloodFill{TCluster,TNode}(TNode,Func{TNode,System.Collections.Generic.IEnumerable{TNode}},List{TNode},TCluster)"/>
        /// </summary>
        /// <typeparam name="TCluster"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="getNeighbors"></param>
        /// <returns> Cluster of all the TNodes that are reachable from <paramref name="startNode"/> directly or transitively by <paramref name="getNeighbors"/></returns>
        public static TCluster FloodFill<TCluster, TNode>(TNode startNode, Func<TNode, IEnumerable<TNode>> getNeighbors) where TCluster : ICluster<TNode>, new()
        {
            var result = new TCluster();
            FloodFill(startNode, getNeighbors, new HashSet<TNode>(), result);
            return result;
        }

        /// <summary>
        /// Recursive FloodFill. In each step, adds the currently exploited node to the visited list and result and exploits all the neighbors
        /// that are not in visited list yet.
        /// </summary>
        /// <typeparam name="TCluster"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="currentNode"></param>
        /// <param name="getNeighbors"></param>
        /// <param name="visited"></param>
        /// <param name="result"></param>
        /// <returns> Cluster of all the TNodes that are reachable from <paramref name="startNode"/> directly or transitively by <paramref name="getNeighbors"/></returns>
        public static TCluster FloodFill<TCluster, TNode>(TNode currentNode, Func<TNode, IEnumerable<TNode>> getNeighbors,
            HashSet<TNode> visited, TCluster result) 
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

        /// <summary>
        /// Splits <paramref name="nodes"/> to the clusters according to relation defined by <paramref name="getNeighbors"/>>
        /// </summary>
        /// <typeparam name="TCluster"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="nodes"></param>
        /// <param name="getNeighbors"></param>
        /// <returns>The list of clusters, each node is exactly in one cluster.</returns>
        public static List<TCluster> GenerateClusters<TCluster, TNode>(IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>> getNeighbors) 
            where TCluster : ICluster<TNode>, new()
        {
            var result = new List<TCluster>();
            var alreadyClustered = new HashSet<TNode>();
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
}
