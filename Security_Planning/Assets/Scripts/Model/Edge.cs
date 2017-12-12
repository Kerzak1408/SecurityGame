using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class Edge
    {
        public Tuple<int, int> OtherIndices;
        public EdgeType Type;
        public float Cost;

        public Edge(int otherI, int otherJ, EdgeType type, float cost)
        {
            OtherIndices = Tuple.New(otherI, otherJ);
            Type = type;
            Cost = cost;
        }
    }
}
