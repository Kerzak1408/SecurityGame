using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class Edge : IAStarEdge<TileNode>
    {
        //public Tuple<int, int> OtherIndices;
        public TileNode Neighbor { get; private set; }
        public EdgeType Type;
        public float Cost { get; private set; }

        public Edge(TileNode neighbor, EdgeType type, float cost)
        {
            Neighbor = neighbor;
            //OtherIndices = Tuple.New(otherI, otherJ);
            Type = type;
            Cost = cost;
        }

        public override string ToString()
        {
            return string.Join(", ", new string[] { Neighbor.Position.ToString(), Type.ToString(), Cost.ToString() });
        }
    }
}
