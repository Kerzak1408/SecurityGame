using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class BaseItem : BaseObject, IPlanningEdgeCreator
    {
        public Vector3 DefaultLocalPosition { get; set; }
        public abstract EdgeType CorrespondingEdgeType { get; }
        public abstract PlanningEdgeType PlanningEdgeType { get; }
        public GameObject Interactable { get; set; }
        public float InteractTime
        {
            get { return 0; }
        }

        public override void StartGame()
        {
            base.StartGame();
            Interactable = gameObject;
        }
        
        public bool ShouldExplore(PlanningNode node)
        {
            return !node.UnlockedEdges.Contains(CorrespondingEdgeType);
        }

        public void ModifyNextNode(PlanningNode node)
        {
            node.UnlockedEdges.Add(CorrespondingEdgeType);
        }

        public void ResetItemPosition()
        {
            transform.localPosition = DefaultLocalPosition;
        }
    }
}
