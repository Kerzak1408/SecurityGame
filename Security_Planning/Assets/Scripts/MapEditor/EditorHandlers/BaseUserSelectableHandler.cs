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
                int optionsCount = dropdownMode.options.Count;
                if (keyCodesPressed.Contains(KeyCode.LeftShift))
                {
                    int decreasedValue = dropdownMode.value - 1;
                    dropdownMode.value = decreasedValue < 0 ? optionsCount - 1 : decreasedValue;
                }
                else
                {
                    dropdownMode.value = (dropdownMode.value + 1) % optionsCount;
                }
                dropdownMode.RefreshShownValue();
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
