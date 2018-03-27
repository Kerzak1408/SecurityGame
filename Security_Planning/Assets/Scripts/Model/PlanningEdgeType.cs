using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Items;

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
