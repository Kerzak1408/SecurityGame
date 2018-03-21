using Assets.Scripts.Model;

namespace Assets.Scripts.Items
{
    public class CardItem : BaseItem
    {
        public override EdgeType CorrespondingEdgeType
        {
            get { return EdgeType.CARD_DOOR; }
        }

        public override PlanningEdgeType PlanningEdgeType
        {
            get { return PlanningEdgeType.CARD; }
        }
    }
}
