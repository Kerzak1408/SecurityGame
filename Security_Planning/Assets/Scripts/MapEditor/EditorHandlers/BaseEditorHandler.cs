using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public abstract class BaseEditorHandler
    {
        protected GridManager gridManager;

        protected BaseEditorHandler(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        public virtual void LeftButtonDown(RaycastHit[] raycastHits) { }
        public virtual void LeftButton(RaycastHit[] raycastHits) { }
        public virtual void LeftButtonUp(RaycastHit[] raycastHits) { }
        public virtual void Scroll(float scroll, RaycastHit[] raycastHits) { }
        public virtual void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed) { }
        public virtual void HoverLogic(RaycastHit[] raycastHits) { }

        public virtual void Start() { }
        public virtual void End() { }
    }
}
