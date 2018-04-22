using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class NewMapHandler : BaseEditorHandler
    {
        private Selectable selectedInputField;

        public NewMapHandler(GridManager gridManager) : base(gridManager)
        {
            selectedInputField = gridManager.InputName;
        }

        public override void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed)
        {
            if (keyCodesDown.Contains(KeyCode.Tab))
            {
                selectedInputField = selectedInputField.FindSelectableOnDown();
                if (selectedInputField == null)
                {
                    selectedInputField = GridManager.InputName;
                }
                selectedInputField.Select();
            }
        }
    }
}
