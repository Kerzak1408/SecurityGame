using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class AddEntityHandler : BaseUserSelectableHandler
    {

        public AddEntityHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            if (raycastHits.Length == 0)
            {
                return;
            }
            RaycastHit firstHit = raycastHits[0];
            gridManager.newEntityPosition = firstHit.point;
            gridManager.PanelEntities.SetActive(true);
            gridManager.Canvas.SetActive(false);
            gridManager.AdjustPanelToCamera(gridManager.PanelEntities);
            gridManager.ChangeEditorHandler<ChooseEntityHandler>();

        }
    }
}
