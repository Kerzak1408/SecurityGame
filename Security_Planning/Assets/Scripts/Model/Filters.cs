using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Model
{
    public static class Filters
    {
        public static Func<TileNode, bool> DetectableFilter(
            IEnumerable<DetectorEntity> deactivatedDetectors=null,
            IEnumerable<TileNode> unlockedNodes = null)
        {
            return node =>
            {
                if (unlockedNodes != null && unlockedNodes.Contains(node))
                {
                    return false;
                }
                return node.IsDetectable(deactivatedDetectors);
            };
        }

        /// <summary>
        /// Filters all <paramref name="hardlyForbiddenTypes"/> and all FENCE, CARD_DOOR and KEY_DOORedges that are not in <paramref name="unlockedEdgeTypes"/>.
        /// In addition, filters also edges that are obstructed, <paramref name="destroyedObstacles"/> determines destroyed detectors that are not considered.
        /// </summary>
        /// <param name="unlockedEdgeTypes"></param>
        /// <param name="hardlyForbiddenTypes"></param>
        /// <param name="destroyedObstacles"></param>
        /// <returns></returns>
        public static Func<TileEdge, bool> EdgeFilter(IEnumerable<EdgeType> unlockedEdgeTypes, 
            IEnumerable<EdgeType> hardlyForbiddenTypes, IEnumerable<BaseEntity> destroyedObstacles)
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
            return edge => edge.IsObstructed(destroyedObstacles) || forbiddenTypes.Contains(edge.Type);
        }
    }
}
