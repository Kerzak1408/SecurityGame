    using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class ChooseEntityHandler : BaseChooseHandler
    {
        public ChooseEntityHandler(GridManager gridManager) : base(gridManager, gridManager.Entities)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            RaycastHit entityHit =
                raycastHits.FirstOrDefault(hit => hit.transform.gameObject.IsEqualToChildOf(gridManager.Entities));
            if (!entityHit.Equals(default(RaycastHit)))
            {
                GameObject hitObject = entityHit.transform.gameObject;
                if (hitObject.IsEqualToChildOf(gridManager.Entities))
                {
                    gridManager.FlagCurrentButton();
                    Object item = ResourcesHolder.Instance.AllEntities.FindByName(hitObject.name);
                    GameObject newObject = gridManager.InstantiateGameObject(item);
                    newObject.DeactivateAllScripts();
                    newObject.DeactivateAllCameras();
                    newObject.name = item.name;
                    Map currentMap = gridManager.GetCurrentMap();
                    List<GameObject> entities = currentMap.Entities;
                    newObject.transform.position = gridManager.newEntityPosition;
                    newObject.transform.parent = currentMap.EmptyParent.transform;
                    newObject.transform.name += ("_" + currentMap.GetNextEntityId());
                    entities.Add(newObject);
                    newObject.GetComponent<BaseEntity>().PrefabName = item.name;
                    gridManager.ChangeEditorHandler<AddEntityHandler>();
                }
            }
            ExitHandler();
        }
    }
}
