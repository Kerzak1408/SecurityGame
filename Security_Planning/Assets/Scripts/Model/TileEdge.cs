using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class TileEdge : Edge<TileNode, EdgeType>
    {
        public TileEdge(TileNode start, TileNode neighbor, EdgeType type, float cost) : base(start, neighbor, type, cost)
        {
        }
    }
}
