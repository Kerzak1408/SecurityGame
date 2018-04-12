using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Algorithms.FloodFill.Results;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class ClusterNode : BaseCluster<TileNode>, IAStarNode<ClusterNode>
    {
        public IntegerTuple Position { get; set; }
        public List<IAStarEdge<ClusterNode>> Edges { get; private set; }

        private Vector3 center = Vector3.back;
        public Vector3 Center
        {
            get
            {
                if (center.Equals(Vector3.back))
                {
                    center = new Vector3(0, 0, 0);
                    foreach (var member in Members)
                    {
                        center += member.WorldPosition;
                    }

                    center /= Members.Count;
                }

                return center;
            }
        }

        public ClusterNode()
        {
            Edges = new List<IAStarEdge<ClusterNode>>();
        }

        public override string ToString()
        {
            if (Members == null || Members.Count == 0)
            {
                return "Empty cluster";
            }

            float xSum = Members.Sum(member => member.Position.First);
            float ySum = Members.Sum(member => member.Position.Second);
            return "cluster center: [" + xSum / Members.Count + ", " + ySum / Members.Count + "]";
        }
    }
}
