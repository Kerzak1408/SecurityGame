using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public abstract class BaseUserSelectableHandler : BaseEditorHandler
    {
        private Dropdown dropdownMode;

        protected BaseUserSelectableHandler(GridManager gridManager) : base(gridManager)
        {
            dropdownMode = gridManager.DropdownMode;
        }

        public override void PressedKeys(params KeyCode[] keyCodes)
        {
            if (keyCodes.Contains(KeyCode.Tab))
            {
                dropdownMode.value = (dropdownMode.value + 1) % dropdownMode.options.Count;
            }
        }

        public override void Scroll(float scroll, RaycastHit[] raycastHits)
        {
            gridManager.DefaultScrollLogic(scroll, raycastHits);
        }
    }
}
