using System;
using System.Linq;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
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

        public override void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed)
        {
            if (keyCodesDown.Contains(KeyCode.Tab))
            {
                bool down = !keyCodesPressed.Contains(KeyCode.LeftShift);
                dropdownMode.ChangeDropdownValue(down);
            }
        }

        public override void Scroll(float scroll, RaycastHit[] raycastHits)
        {
            gridManager.DefaultScrollLogic(scroll, raycastHits);
        }


        public Func<RaycastHit, bool> HasScriptOfTypePredicate<T>() where T : MonoBehaviour
        {
            return hit => hit.transform.gameObject.HasScriptOfType<T>();
        }
    }
}
