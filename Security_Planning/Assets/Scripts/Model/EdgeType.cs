using System.Collections.Generic;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Model
{
    public enum EdgeType
    {
        NORMAL, DOOR, WINDOW, KEY_DOOR, CARD_DOOR, FENCE, NONE
    }

    public static class EdgeTypeUtils
    {
        public static IEnumerable<EdgeType> BannableEdgeType
        {
            get { return new[] { EdgeType.FENCE, EdgeType.WINDOW }; }
        }

        public static EdgeType ParseString(string tileName)
        {
            if (tileName.Contains(Map.WINDOW))
            {
                return EdgeType.WINDOW;
            }
            if (tileName.Contains(Map.FENCE))
            {
                return EdgeType.FENCE;
            }
            if (tileName.Contains(Map.KEY_GATE) ||
                tileName.Contains(Map.ROLLING_DOOR))
            {
                return EdgeType.KEY_DOOR;
            }
            if (tileName.Contains(Map.GATE))
            {
                return EdgeType.DOOR;
            }
            return EdgeType.NONE;
        }
    }
}