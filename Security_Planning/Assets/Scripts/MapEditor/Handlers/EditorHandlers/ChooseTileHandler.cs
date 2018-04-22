using System.Linq;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class ChooseTileHandler : BaseChooseHandler
    {
        public ChooseTileHandler(GridManager gridManager) : base(gridManager, gridManager.Tiles)
        {
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            System.Func<RaycastHit, bool> isTile = x => x.transform.parent == GridManager.Tiles.transform;
            if (raycastHits.Any(isTile))
            {
                // Choose the gameobject that is in the panel of tiles.
                GameObject hitObject = raycastHits.FirstOrDefault(isTile).transform.gameObject;
                GridManager.FlagCurrentButton();
                Map map = GridManager.GetCurrentMap();
                GameObject[,] currentGrid = map.Tiles;
                Object item = ResourcesHolder.Instance.AllTiles.FindByName(hitObject.name);

                GameObject newObject = GridManager.InstantiateGameObject(item);
                newObject.DeactivateAllScripts();
                newObject.DeactivateAllCamerasAndAudioListeners();
                
                
                Tuple<int, int> coords = currentGrid.GetIndices(GridManager.ClickedTile);
                if (newObject.HasScriptOfType<PasswordGate>())
                {
                    map.PasswordDictionary[coords] = newObject.GetComponent<PasswordGate>().Password;
                }
                newObject.name = item.name;
                newObject.transform.position = GridManager.ClickedTile.transform.position;
                newObject.transform.parent = GridManager.ClickedTile.transform.parent;
                currentGrid[coords.First, coords.Second] = newObject;
                GridManager.DestroyGameObject(GridManager.ClickedTile);
            }
            ExitHandler();
        }
    }
}
