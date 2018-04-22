using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Assignable to UI element to enable its Drag n Drop.
    /// </summary>
    public class DragAndDropUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        private Vector3 previousMousePosition;

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - previousMousePosition;
            previousMousePosition = currentMousePosition;
            if (Input.GetMouseButton(0))
            {
                transform.position += mouseDelta;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            previousMousePosition = Input.mousePosition;
        }
    }
}
