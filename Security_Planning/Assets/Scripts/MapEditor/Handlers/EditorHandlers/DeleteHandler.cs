using System.Linq;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class DeleteHandler : BaseUserSelectableHandler
    {
        public DeleteHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            RaycastHit entityHit = raycastHits.FirstOrDefault(hit =>
                GridManager.GetCurrentMap().Entities.Contains(hit.transform.gameObject));
            if (!entityHit.Equals(default(RaycastHit)))
            {
                GameObject hitObject = entityHit.transform.gameObject;

                if (GridManager.GetCurrentMap().Entities.Contains(hitObject) && !hitObject.HasScriptOfType<BaseCharacter>())
                {
                    GridManager.ButtonRemoveEntity.SetActive(true);
                    GridManager.ButtonRemoveEntity.transform.position = Input.mousePosition;
                    GridManager.ToBeRemovedEntity = hitObject;
                }
                else
                {
                    CancelRemoving();
                }
            
            }
            else
            {
                CancelRemoving();
            }
        }

        public override void Scroll(float scroll, RaycastHit[] raycastHits)
        {
            CancelRemoving();
            base.Scroll(scroll, raycastHits);
        }

        public override void End()
        {
            CancelRemoving();
        }

        private void CancelRemoving()
        {
            GridManager.ButtonRemoveEntity.SetActive(false);
            GridManager.ToBeRemovedEntity = null;
        }
    }
}
