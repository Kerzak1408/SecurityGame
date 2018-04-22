using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    public enum PlanningEdgeType
    {
        KEY, WIRE_CUTTER, CARD, MONEY, CAMERA, PIR
    }

    public static class PlanningEdgeTypeUtils
    {
        public static IEnumerable<PlanningEdgeType> BannableEdgeTypes
        {
            get { return new List<PlanningEdgeType>() {PlanningEdgeType.CAMERA, PlanningEdgeType.PIR}; }
        }
    }
}
