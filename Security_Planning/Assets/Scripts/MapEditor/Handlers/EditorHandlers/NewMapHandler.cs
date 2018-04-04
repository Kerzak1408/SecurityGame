using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class NewMapHandler : BaseEditorHandler
    {
        private Selectable SelectedInputField;

        public NewMapHandler(GridManager gridManager) : base(gridManager)
        {
            SelectedInputField = gridManager.InputName;
        }

        public override void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed)
        {
            if (keyCodesDown.Contains(KeyCode.Tab))
            {
                SelectedInputField = SelectedInputField.FindSelectableOnDown();
                if (SelectedInputField == null)
                {
                    SelectedInputField = gridManager.InputName;
                }
                SelectedInputField.Select();
            }
        }
    }
}
