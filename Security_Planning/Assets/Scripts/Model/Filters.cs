using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Model
{
    public static class Filters
    {
        public static Func<TileNode, bool> DetectableFilter(IEnumerable<DetectorEntity> deactivatedDetectors=null)
        {
            return node => node.IsDetectable(deactivatedDetectors);
        } 

        public static Func<TileEdge, bool> EdgeFilter(IEnumerable<EdgeType> unlockedEdgeTypes=null, IEnumerable<EdgeType> hardlyForbiddenTypes=null)
        {
            HashSet<EdgeType> forbiddenTypes = new HashSet<EdgeType> {EdgeType.FENCE, EdgeType.CARD_DOOR, EdgeType.KEY_DOOR};
            if (unlockedEdgeTypes != null)
            {
                foreach (EdgeType unlockedType in unlockedEdgeTypes)
                {
                    forbiddenTypes.Remove(unlockedType);
                }
            }

            if (hardlyForbiddenTypes != null)
            {
                foreach (EdgeType forbiddenType in hardlyForbiddenTypes)
                {
                    forbiddenTypes.Add(forbiddenType);
                }
            }
            return edge => forbiddenTypes.Contains(edge.Type);
        }
    }
}
