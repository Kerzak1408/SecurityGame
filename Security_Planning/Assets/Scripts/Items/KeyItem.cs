using Assets.Scripts.Model;

namespace Assets.Scripts.Items
{
    public class KeyItem : BaseItem
    {
        public override EdgeType CorrespondingEdgeType
        {
            get { return EdgeType.KEY_DOOR; }
        }

        public override PlanningEdgeType PlanningEdgeType
        {
            get { return PlanningEdgeType.KEY; }
        }
    }
}
