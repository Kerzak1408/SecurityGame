using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class EditParametersHandler : BaseUserSelectableHandler
    {
        public EditParametersHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void LeftButton(RaycastHit[] raycastHits)
        {
            if (raycastHits.Length != 0)
            {
                GameObject hitObject = raycastHits[0].transform.gameObject;
                if (hitObject.transform.parent.gameObject.HasScriptOfType<PasswordGate>())
                {
                    gridManager.currentPasswordGate = hitObject.GetComponentInParent<PasswordGate>();
                    gridManager.PanelPassword.GetComponentInChildren<InputField>().text = gridManager.currentPasswordGate.Password;
                    var currentMap = gridManager.GetCurrentMap();
                    gridManager.passwordIndices = currentMap.Tiles.GetIndices(hitObject.transform.parent.gameObject);
                    currentMap.PasswordDictionary[gridManager.passwordIndices] = gridManager.currentPasswordGate.Password;
                    gridManager.Grids.SetActive(true);
                    gridManager.PanelPassword.SetActive(true);
                }
            }
        }
    }
}
