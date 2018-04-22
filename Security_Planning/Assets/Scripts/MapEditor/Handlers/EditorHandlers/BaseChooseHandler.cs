using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    /// <summary>
    /// Base class for choosing from the panels.
    /// </summary>
    public abstract class BaseChooseHandler : BaseEditorHandler
    {
        private readonly GameObject scrolledObject;
        private readonly float cameraOriginalSize;

        protected BaseChooseHandler(GridManager gridManager, GameObject scrolledObject) : base(gridManager)
        {
            this.scrolledObject = scrolledObject;
            cameraOriginalSize = gridManager.CameraOriginalSize;
        }

        public override void Start()
        {
            base.Start();
            GridManager.SetCanvasActive(false);
            GridManager.Grids.SetActive(false);
            GridManager.PanelInfo.SetActive(true);
        }

        public override void Scroll(float scroll, RaycastHit[] raycastHits)
        {
            if (raycastHits.Length != 0)
            {
                scrolledObject.transform.position += new Vector3(Camera.main.orthographicSize / cameraOriginalSize * 25 * scroll, 0, 0);
            }
        }

        protected void ExitHandler()
        {
            GridManager.Panel.SetActive(false);
            GridManager.PanelEntities.SetActive(false);
            GridManager.ActivatePreviousHandler();
        }

        public override void End()
        {
            GridManager.SetCanvasActive(true);
            GridManager.Grids.SetActive(true);
        }
    }
}
