using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public static class Filters
    {
        public static Func<TileNode, bool> DetectableFilter = node => node.IsDetectable();

        public static Func<TileEdge, bool> EdgeFilter(IEnumerable<EdgeType> unlockedEdgeTypes=null)
        {
            List<EdgeType> forbiddenTypes = new List<EdgeType> {EdgeType.FENCE, EdgeType.CARD_DOOR, EdgeType.KEY_DOOR};
            if (unlockedEdgeTypes != null)
            {
                foreach (EdgeType unlockedType in unlockedEdgeTypes)
                {
                    forbiddenTypes.Remove(unlockedType);
                }
            }

            return edge => forbiddenTypes.Contains(edge.Type);
        }
    }
}
