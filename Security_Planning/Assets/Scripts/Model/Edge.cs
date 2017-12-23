using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class Edge : IAStarEdge
    {
        //public Tuple<int, int> OtherIndices;
        public TileNode Neighbor { get; private set; }
        public EdgeType Type;
        public float Cost { get; private set; }

        IAStarNode IAStarEdge.Neighbor
        {
            get { return Neighbor; }
        }

        public Edge(TileNode neighbor, EdgeType type, float cost)
        {
            Neighbor = neighbor;
            //OtherIndices = Tuple.New(otherI, otherJ);
            Type = type;
            Cost = cost;
        }
    }
}
