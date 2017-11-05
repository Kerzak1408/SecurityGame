using UnityEngine;
using System.Linq;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class DragHandler : BaseUserSelectableHandler
    {
        private GameObject draggedObject;

        public DragHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void LeftButtonDown(RaycastHit[] raycastHits)
        {
            if (raycastHits.Length != 0)
            {
                Map map = gridManager.GetCurrentMap();
                // Get Entity collider.
                RaycastHit entityHit = raycastHits.FirstOrDefault(x => map.Entities.Contains(x.transform.gameObject));
                if (!entityHit.Equals(default(RaycastHit)))
                {
                    GameObject hitObject = entityHit.transform.gameObject;
                    draggedObject = hitObject;
                }


            }
        }

        public override void LeftButton(RaycastHit[] raycastHits)
        {
            if (draggedObject != null)
            {
                gridManager.FlagCurrentButton();
                Vector3 previousPosition = draggedObject.transform.position;
                var v3 = Input.mousePosition;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                v3.z = previousPosition.z;
                draggedObject.transform.position = v3;
            }
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            draggedObject = null;
        }
    }
}
