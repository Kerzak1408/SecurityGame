using Assets.Scripts.Model;

namespace Assets.Scripts.Items
{
    public class WireCutterItem : BaseItem
    {
        public override EdgeType CorrespondingEdgeType
        {
            get { return EdgeType.FENCE; }
        }

        public override PlanningEdgeType PlanningEdgeType
        {
            get { return PlanningEdgeType.WIRE_CUTTER; }
        }
    }
}
