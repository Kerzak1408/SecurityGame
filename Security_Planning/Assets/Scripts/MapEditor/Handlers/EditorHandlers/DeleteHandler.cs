using System.Linq;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using Assets.Scripts.MapEditor;
using Assets.Scripts.MapEditor.EditorHandlers;
using UnityEngine;

public class DeleteHandler : BaseUserSelectableHandler
{
    public DeleteHandler(GridManager gridManager) : base(gridManager)
    {
    }

    public override void LeftButtonUp(RaycastHit[] raycastHits)
    {
        RaycastHit entityHit = raycastHits.FirstOrDefault(hit =>
            gridManager.GetCurrentMap().Entities.Contains(hit.transform.gameObject));
        if (!entityHit.Equals(default(RaycastHit)))
        {
            GameObject hitObject = entityHit.transform.gameObject;

            if (gridManager.GetCurrentMap().Entities.Contains(hitObject) && !hitObject.HasScriptOfType<BaseCharacter>())
            {
                gridManager.ButtonRemoveEntity.SetActive(true);
                gridManager.ButtonRemoveEntity.transform.position = Input.mousePosition;
                gridManager.toBeRemovedEntity = hitObject;
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
        gridManager.ButtonRemoveEntity.SetActive(false);
        gridManager.toBeRemovedEntity = null;
    }
}
