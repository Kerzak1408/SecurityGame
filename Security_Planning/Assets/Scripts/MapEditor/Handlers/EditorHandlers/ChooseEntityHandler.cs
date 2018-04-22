using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class ChooseEntityHandler : BaseChooseHandler
    {
        public ChooseEntityHandler(GridManager gridManager) : base(gridManager, gridManager.Entities)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            RaycastHit entityHit =
                raycastHits.FirstOrDefault(hit => hit.transform.gameObject.IsEqualToChildOf(GridManager.Entities));
            if (!entityHit.Equals(default(RaycastHit)))
            {
                GameObject hitObject = entityHit.transform.gameObject;
                if (hitObject.IsEqualToChildOf(GridManager.Entities))
                {
                    GridManager.FlagCurrentButton();
                    Object item = ResourcesHolder.Instance.AllEntities.FindByName(hitObject.name);
                    GameObject newObject = GridManager.InstantiateGameObject(item);
                    newObject.DeactivateAllScripts();
                    newObject.DeactivateAllCamerasAndAudioListeners();
                    newObject.name = item.name;
                    Map currentMap = GridManager.GetCurrentMap();
                    List<GameObject> entities = currentMap.Entities;
                    newObject.transform.position = GridManager.NewEntityPosition;
                    newObject.transform.parent = currentMap.EmptyParent.transform;
                    newObject.transform.name += ("_" + currentMap.GetNextEntityId());
                    entities.Add(newObject);
                    newObject.GetComponent<BaseEntity>().PrefabName = item.name;
                }
            }
            ExitHandler();
        }

    }
}
