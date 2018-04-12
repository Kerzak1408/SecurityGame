using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public static class Filters
    {
        public static Func<TileNode, bool> DetectableFilter(
            IEnumerable<DetectorEntity> deactivatedDetectors=null,
            IEnumerable<DetectorType> ignoredTypes = null,
            IEnumerable<TileNode> unlockedNodes = null)
        {
            return node =>
            {
                if (unlockedNodes != null && unlockedNodes.Contains(node))
                {
                    return false;
                }
                return node.IsDetectable(deactivatedDetectors, ignoredTypes);
            };
        } 

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
