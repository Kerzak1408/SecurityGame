using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class BaseItem : MonoBehaviour
    {
        public Vector3 DefaultLocalPosition { get; set; }

        public void ResetItemPosition()
        {
            transform.localPosition = DefaultLocalPosition;
        }
    }
}
