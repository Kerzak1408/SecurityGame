using System;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class ReplaceTileHandler : BaseUserSelectableHandler
    {

        private GameObject hoveredObject;
        private Color hoveredObjectOriginalColor;
        private Color hoveredObjectOriginalEmissionColor;

        public ReplaceTileHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            Map map = GridManager.GetCurrentMap();
            RaycastHit tileHit = raycastHits.FirstOrDefault(x => map.Tiles.Contains(x.transform.gameObject));
            if (!tileHit.Equals(default(RaycastHit)))
            {
                GameObject hitObject = tileHit.transform.gameObject;
                HoverEnded();
                
                if (hitObject.transform.parent.gameObject.HasScriptOfType<PasswordGate>())
                {
                    GridManager.CurrentPasswordGate = hitObject.GetComponentInParent<PasswordGate>();
                    GridManager.PanelPassword.GetComponentInChildren<InputField>().text = GridManager.CurrentPasswordGate.Password;
                    var currentMap = GridManager.GetCurrentMap();
                    Tuple<int, int> passwordIndices = currentMap.Tiles.GetIndices(hitObject.transform.parent.gameObject);
                    map.PasswordDictionary[passwordIndices] = GridManager.CurrentPasswordGate.Password;
                    GridManager.Grids.SetActive(true);
                    GridManager.PanelPassword.SetActive(true);
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
                    hitObject.ChangeAlbedoAndEmissionColor(Color.red);
                    GridManager.ClickedTile = hitObject;
                    GridManager.AdjustPanelToCamera(GridManager.Panel);
                    GridManager.Panel.SetActive(true);
                    GridManager.ChangeEditorHandler<ChooseTileHandler>();
                }

            }
        }

        public override void HoverLogic(RaycastHit[] raycastHits)
        {
            Map currentMap = GridManager.GetCurrentMap();
            if (!GridManager.Panel.activeInHierarchy)
            {
                RaycastHit tileHit = raycastHits.FirstOrDefault(x => currentMap.Tiles.Contains(x.transform.gameObject));
                if (!tileHit.Equals(default(RaycastHit)))
                {
                    GameObject hitObject = tileHit.transform.gameObject;
                    if (hitObject != hoveredObject)
                    {
                        HoverEnded();
                        var currentParent = currentMap.EmptyParent;
                        if (hitObject.IsEqualToChildOf(currentParent) ||
                            currentMap.Entities.Contains(hitObject)
                        )
                        {
                            hoveredObject = hitObject;
                            Material material = hitObject.GetComponent<Renderer>().material;
                            hoveredObjectOriginalColor = material.color;
                            if (material.HasProperty("_EmissionColor"))
                            {
                                hoveredObjectOriginalEmissionColor = material.GetColor("_EmissionColor");
                            }
                            hitObject.ChangeAlbedoAndEmissionColor(Color.green);
                        }
                    }
                }
                else
                {
                    HoverEnded();
                    hoveredObject = null;
                }
            }
            base.HoverLogic(raycastHits);
        }

        private void HoverEnded()
        {
            if (hoveredObject != null)
            {
                hoveredObject.ChangeColors(hoveredObjectOriginalColor, hoveredObjectOriginalEmissionColor);
            }
        }

        public override void End()
        {
            HoverEnded();
        }
    }
}
