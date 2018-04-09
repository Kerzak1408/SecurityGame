using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Algorithms.FloodFill.Results
{
    public abstract class BaseCluster<TNode> : ICluster<TNode>
    {
        public List<TNode> Members { get; set; }

        protected BaseCluster()
        {
            Members = new List<TNode>();
        }
    }
}
