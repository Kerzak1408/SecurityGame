using System;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class ReplaceTileHandler : BaseUserSelectableHandler
    {

        private GameObject hoveredObject;
        private Color hoveredObjectOriginalColor;

        public ReplaceTileHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            Map map = gridManager.GetCurrentMap();
            RaycastHit tileHit = raycastHits.FirstOrDefault(x => map.Tiles.Contains(x.transform.gameObject));
            if (!tileHit.Equals(default(RaycastHit)))
            {
                GameObject hitObject = tileHit.transform.gameObject;
                HoverEnded();
                
                if (hitObject.transform.parent.gameObject.HasScriptOfType<PasswordGate>())
                {
                    gridManager.currentPasswordGate = hitObject.GetComponentInParent<PasswordGate>();
                    gridManager.PanelPassword.GetComponentInChildren<InputField>().text = gridManager.currentPasswordGate.Password;
                    var currentMap = gridManager.GetCurrentMap();
                    Tuple<int, int> passwordIndices = currentMap.Tiles.GetIndices(hitObject.transform.parent.gameObject);
                    map.PasswordDictionary[passwordIndices] = gridManager.currentPasswordGate.Password;
                    gridManager.Grids.SetActive(true);
                    gridManager.PanelPassword.SetActive(true);
                }
                else if (hitObject.HasScriptOfType<TransmitterEntity>())
                {
                    var transmitterEntity = hitObject.GetComponent<TransmitterEntity>();
                    Type receiverType = transmitterEntity.GetReceiverType();
                    foreach (GameObject entity in map.Entities)
                    {
                        entity.SetActive(entity.HasScriptOfType(receiverType));
                    }
                }
                else
                {
                    hitObject.ChangeColor(Color.red);
                    gridManager.ClickedObject = hitObject;
                    gridManager.Canvas.SetActive(false);
                    gridManager.AdjustPanelToCamera(gridManager.Panel);
                    gridManager.Panel.SetActive(true);
                    gridManager.ChangeEditorHandler<ChooseTileHandler>();
                }

            }
        }

        public override void HoverLogic(RaycastHit[] raycastHits)
        {
            Map currentMap = gridManager.GetCurrentMap();
            if (!gridManager.Panel.activeInHierarchy)
            {
                RaycastHit tileHit = raycastHits.FirstOrDefault(x => currentMap.Tiles.Contains(x.transform.gameObject));
                if (!tileHit.Equals(default(RaycastHit)))
                {
                    GameObject HitObject = tileHit.transform.gameObject;
                    if (HitObject != hoveredObject)
                    {
                        HoverEnded();
                        var currentParent = currentMap.EmptyParent;
                        if (HitObject.IsEqualToChildOf(currentParent) ||
                            /*HitObject.transform.parent.gameObject.HasScriptOfType<PasswordGate>()*/
                            currentMap.Entities.Contains(HitObject)
                        )
                        {
                            hoveredObject = HitObject;
                            hoveredObjectOriginalColor = HitObject.GetComponent<Renderer>().material.color;
                            HitObject.ChangeColor(Color.green);
                        }
                    }
                }
                else
                {
                    HoverEnded();
                    hoveredObject = null;
                }
            }

        }

        private void HoverEnded()
        {
            if (hoveredObject != null)
            {
                hoveredObject.ChangeColor(hoveredObjectOriginalColor);
            }
        }

        public override void End()
        {
            HoverEnded();
        }
    }
}
