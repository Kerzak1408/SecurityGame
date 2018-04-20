using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public abstract class BaseChooseHandler : BaseEditorHandler
    {
        private GameObject scrolledObject;
        public float cameraOriginalSize;
        protected BaseChooseHandler(GridManager gridManager, GameObject scrolledObject) : base(gridManager)
        {
            this.scrolledObject = scrolledObject;
            this.cameraOriginalSize = gridManager.cameraOriginalSize;
        }

        public override void Start()
        {
            base.Start();
            gridManager.SetCanvasActive(false);
            gridManager.Grids.SetActive(false);
            gridManager.PanelInfo.SetActive(true);
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
            gridManager.Panel.SetActive(false);
            gridManager.PanelEntities.SetActive(false);
            gridManager.ActivatePreviousHandler();
        }

        public override void End()
        {
            gridManager.SetCanvasActive(true);
            gridManager.Grids.SetActive(true);
        }
    }
}
