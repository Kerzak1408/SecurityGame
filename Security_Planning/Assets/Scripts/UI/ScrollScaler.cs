using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class ScrollScaler : MonoBehaviour
    {
        private static readonly float MAX_SCALE = 2;
        private static readonly float MIN_SCALE = 0.1f;

        public void ScrollScale(BaseEventData e)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Vector3 potentialScale = transform.localScale + Vector3.one * scroll;
            if (potentialScale.x < MAX_SCALE && potentialScale.x > MIN_SCALE)
            {
                transform.localScale = potentialScale;
            }
        }
    }
}
