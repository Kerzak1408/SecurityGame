using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class BaseItem : MonoBehaviour
    {
        public Vector3 DefaultLocalPosition { get; set; }

        public abstract EdgeType CorrespondingEdgeType { get; }

        public abstract PlanningEdgeType PlanningEdgeType { get; }

        public void ResetItemPosition()
        {
            transform.localPosition = DefaultLocalPosition;
        }
    }
}
