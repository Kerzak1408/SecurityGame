using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class ChooseTileHandler : BaseChooseHandler
    {

        
        public ChooseTileHandler(GridManager gridManager) : base(gridManager, gridManager.Tiles)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            System.Func<RaycastHit, bool> isTile = x => x.transform.parent == gridManager.Tiles.transform;
            if (raycastHits.Any(isTile))
            {
                // Choose the gameobject that is in the panel of tiles.
                GameObject hitObject = raycastHits.FirstOrDefault(isTile).transform.gameObject;
                gridManager.FlagCurrentButton();
                Map map = gridManager.GetCurrentMap();
                GameObject[,] currentGrid = map.Tiles;
                Object item = ResourcesHolder.Instance.AllTiles.FindByName(hitObject.name);

                GameObject newObject = gridManager.InstantiateGameObject(item);
                newObject.DeactivateAllScripts();
                newObject.DeactivateAllCameras();
                
                
                Tuple<int, int> coords = currentGrid.GetIndices(gridManager.ClickedTile);
                if (newObject.HasScriptOfType<PasswordGate>())
                {
                    map.PasswordDictionary[coords] = newObject.GetComponent<PasswordGate>().Password;
                }
                newObject.name = item.name;
                newObject.transform.position = gridManager.ClickedTile.transform.position;
                newObject.transform.parent = gridManager.ClickedTile.transform.parent;
                currentGrid[coords.First, coords.Second] = newObject;
                gridManager.DestroyGameObject(gridManager.ClickedTile);
            }
            ExitHandler();
        }


    }
}
