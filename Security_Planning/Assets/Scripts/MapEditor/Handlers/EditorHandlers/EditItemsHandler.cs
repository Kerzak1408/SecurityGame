using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class EditItemsHandler : BaseUserSelectableHandler
    {
        private Map currentMap;
        private BaseCharacter selectedCharacter;
        private List<Object> currentItemObjects;

        private readonly GameObject panelAllItems;
        private readonly GameObject allItems;

        private readonly GameObject panelItems;
        private readonly GameObject panelItemsStart;
        private readonly GameObject items;

        private void RefreshItems(RaycastHit[] raycastHits, System.Func<RaycastHit, bool> predicate, Action<List<Object>, Object> action)
        {
            RaycastHit hitItem = raycastHits.First(predicate);
            string itemName = hitItem.transform.name;
            action(currentItemObjects, ResourcesHolder.Instance.AllItemsIcons.First(item => item.name == itemName));
            selectedCharacter.Data.ItemNames = currentItemObjects.Select(item => item.name).ToArray();
            Vector3 startPosition = items.transform.childCount == 0
                ? panelItemsStart.transform.position
                : items.transform.GetChild(0).transform.position;
            GridManager.InitializePanelGroup(currentItemObjects.ToArray(), startPosition, items);
            GridManager.FlagCurrentButton();
        }

        public EditItemsHandler(GridManager gridManager) : base(gridManager)
        {
            panelAllItems = gridManager.PanelAllItems;
            var panelAllItemsStart = gridManager.PanelAllItemsStart;
            allItems = gridManager.AllItems;

            panelItems = gridManager.PanelItems;
            panelItemsStart = gridManager.PanelItemsStart;
            items = gridManager.Items;

            gridManager.InitializePanelGroup(ResourcesHolder.Instance.AllItemsIcons, panelAllItemsStart.transform.position, allItems);
        }

        public override void Start()
        {
            currentMap = GridManager.GetCurrentMap();
            currentMap.DeactivateEntitiesExceptOfType(typeof(BaseCharacter));
            GridManager.SetCanvasActive(false);
            GridManager.DropdownMode.gameObject.SetActive(true);
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            if (selectedCharacter != null)
            {
                Func<RaycastHit, bool> addItemPredicate = hit => hit.transform.gameObject.IsChildOf(allItems);
                Func<RaycastHit, bool> removeItemPredicate = hit => hit.transform.gameObject.IsChildOf(items);
                if (raycastHits.Any(addItemPredicate))
                {
                    RefreshItems(raycastHits, addItemPredicate, (list, obj) => list.Add(obj));
                }
                else if (raycastHits.Any(removeItemPredicate))
                {
                    RefreshItems(raycastHits, removeItemPredicate, (list, obj) => list.Remove(obj));
                }
                else
                {
                    currentItemObjects = null;
                    selectedCharacter = null;
                    panelItems.SetActive(false);
                    panelAllItems.SetActive(false);
                    GridManager.SetCanvasActive(true);
                }

            }
            else if (raycastHits.Any(HasScriptOfTypePredicate<BaseCharacter>()))
            {
                RaycastHit characterHit = raycastHits.First(HasScriptOfTypePredicate<BaseCharacter>());
                selectedCharacter = characterHit.transform.gameObject.GetComponent<BaseCharacter>();
                string[] itemNames = selectedCharacter.Data.ItemNames;
                Object[] allItemsIcons = ResourcesHolder.Instance.AllItemsIcons;
                foreach (Object icon in allItemsIcons)
                {
                    Debug.Log("all items: " + icon.name);
                }
                foreach (string itemName in itemNames)
                {
                    Debug.Log("itemName: " + itemName);
                }
                currentItemObjects = itemNames.Select(itemName => allItemsIcons.First(icon => icon.name == itemName)).ToList();
                GridManager.InitializePanelGroup(currentItemObjects.ToArray(), panelItemsStart.transform.position, items);
                GridManager.AdjustPanelToCamera(panelAllItems, 3);
                GridManager.AdjustPanelToCamera(panelItems, -3);
                panelItems.SetActive(true);
                panelAllItems.SetActive(true);
            }
        }

        public override void End()
        {
            currentMap.ActivateAllEntities();
            GridManager.SetCanvasActive(true);
        }

        public override void Scroll(float scroll, RaycastHit[] raycastHits)
        {
            if (selectedCharacter == null)
            {
                base.Scroll(scroll, raycastHits);
            }
            else
            {
                if (raycastHits.Any(hit => hit.transform.gameObject.Equals(panelAllItems)))
                {
                    allItems.transform.position += new Vector3(Camera.main.orthographicSize / GridManager.CameraOriginalSize * 25 * scroll, 0, 0);
                }
                else if (raycastHits.Any(hit => hit.transform.gameObject.Equals(panelItems)))
                {
                    items.transform.position += new Vector3(Camera.main.orthographicSize / GridManager.CameraOriginalSize * 25 * scroll, 0, 0);
                }
            }
        }
    }
}
