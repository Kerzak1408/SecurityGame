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

        public override void PressedKeys(params KeyCode[] keyCodes)
        {
            if (keyCodes.Contains(KeyCode.Tab))
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
