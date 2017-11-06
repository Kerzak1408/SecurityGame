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

        public virtual void LeftButtonDown(RaycastHit[] raycastHitsHits) { }
        public virtual void LeftButton(RaycastHit[] raycastHitsHits) { }
        public virtual void LeftButtonUp(RaycastHit[] raycastHitsHits) { }
        public virtual void Scroll(float scroll, RaycastHit[] raycastHitsHits) { }
        public virtual void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed) { }
        public virtual void HoverLogic(RaycastHit[] raycastHitsHits) { }

        public virtual void Start() { }
        public virtual void End() { }
    }
}
