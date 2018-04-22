using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
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
            GridManager.NewEntityPosition = firstHit.point;
            GridManager.PanelEntities.SetActive(true);
            GridManager.AdjustPanelToCamera(GridManager.PanelEntities);
            GridManager.ChangeEditorHandler<ChooseEntityHandler>();
        }
    }
}
