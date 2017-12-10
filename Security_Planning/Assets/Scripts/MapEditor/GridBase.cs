using System;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class GridBase : MonoBehaviour
    {
        private static readonly string UP = "Up";
        private static readonly string DOWN = "Down";
        private static readonly string LEFT = "Left";
        private static readonly string RIGHT = "Right";
        private static readonly string FENCE = "Fence";

        protected string MAPS_PATH;

        protected const string TILES = "Tiles";
        protected const string ENTITIES = "Entities";
        protected const string PASSWORDS = "Passwords";

        protected AIModel model;

        // Use this for initialization
        protected virtual void Start () {
            MAPS_PATH = FileHelper.JoinPath(Application.persistentDataPath, "Maps");
            Physics.gravity = Vector3.forward;
        }

        // Update is called once per frame
        void Update () {
		
        }

        protected virtual Map LoadMap(string mapName, UnityEngine.UI.Button correspondingButton = null, bool mapVisible = false)
        {
            mapName =  mapName.Replace(' ', '_');
            var serializer = Serializer.Instance;
            var namesMatrix = serializer.Deserialize<string[,]>(FileHelper.JoinPath(MAPS_PATH, mapName, TILES));
            var entitiesData = serializer.Deserialize<List<Tuple<string, BaseEntityData>>>(FileHelper.JoinPath(MAPS_PATH, mapName, ENTITIES));
            var passwordDictionary = serializer.Deserialize<Dictionary<Tuple<int, int>, string>>(FileHelper.JoinPath(MAPS_PATH, mapName, PASSWORDS));
            var allTiles = ResourcesHolder.Instance.AllTiles;

            foreach (KeyValuePair<Tuple<int, int>, string> kvPair in passwordDictionary)
            {
                Debug.Log("KEY=" + kvPair.Key + " VALUE=" + kvPair.Value);
            }

            int width = namesMatrix.GetLength(1);
            int height = namesMatrix.GetLength(0);
            var loadedGrid = new GameObject[height, width];
            GameObject emptyParent = new GameObject();
            emptyParent.SetActive(mapVisible);
            for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                string currentName = namesMatrix[i, j].Trim();
                Debug.Log("Loading tile = +" + currentName + "+");
                var newTile = allTiles.FindByName(currentName);
                GameObject newObject = Instantiate(newTile, transform) as GameObject;
                loadedGrid[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.name = newTile.name;
                newObject.transform.parent = emptyParent.transform;
                if (newObject.HasScriptOfType<PasswordGate>())
                {
                    var passwordGateScript = newObject.GetComponent<PasswordGate>();
                    var password = passwordDictionary[Tuple.New(i, j)];
                    passwordGateScript.Password = password;
                }
            }

            var allEntities = ResourcesHolder.Instance.AllEntities;
            var entities = new List<GameObject>();
            foreach (Tuple<string, BaseEntityData> kvPair in entitiesData)
            {
                var currentName = kvPair.First;
                var newEntity = allEntities.FindByName(currentName);
                GameObject newObject = Instantiate(newEntity, transform) as GameObject;
                newObject.name = newEntity.name;
                newObject.GetComponent<BaseEntity>().Deserialize(kvPair.Second);
                newObject.transform.parent = emptyParent.transform;
                entities.Add(newObject);
            }
            return new Map(loadedGrid, entities, emptyParent, passwordDictionary);
        }

        protected void GenerateCeiling(GameObject[,] grid, Transform parent)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            bool?[,] ceilingGrid = new bool?[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bool? currentTile = ceilingGrid[i, j];
                    if (!currentTile.HasValue)
                    {
                        FloodFillResult result = new FloodFillResult();
                        Debug.Log("-------------------------------- Flood fill begun --------------------------------");
                        FloodFill(i, j, grid, result, new List<Vector2>());
                        Debug.Log("-------------------------------- Flood fill ended --------------------------------");
                        foreach (Vector2 coordinate in result.Coordinates)
                        {
                            ceilingGrid[(int) coordinate.x, (int) coordinate.y] = result.IsRoom;
                        }
                        if (result.IsRoom)
                        {
                            AddCeiling(grid, result.Coordinates, parent);
                            AddPavementTexture(grid, result.Coordinates, "InnerFloor");
                        }
                        else
                        {
                            AddPavementTexture(grid, result.Coordinates, "Pavement");
                        }
                    }
                }
            }
        }

        private void AddPavementTexture(GameObject[,] grid, List<Vector2> resultCoordinates, string textureName)
        {
            Sprite texture = Resources.Load<Sprite>("Textures/" + textureName);
            texture = Resources.Load<Sprite>("Textures/" + textureName);
            foreach (Vector2 coordinate in resultCoordinates)
            {
                GameObject tile = grid[(int) coordinate.x, (int) coordinate.y];
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                    spriteRenderer.sprite = texture;
                }

            }
        }

        private void AddCeiling(GameObject[,] grid, List<Vector2> coordinates, Transform parent)
        {
            Color color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            foreach (Vector2 coordinate in coordinates)
            {
                GameObject ceilingTile =
                    Instantiate(ResourcesHolder.Instance.AllTiles.FindByName(GridManager.EMPTY_SQUARE)) as GameObject;
                ceilingTile.transform.position = grid[(int) coordinate.x, (int) coordinate.y].transform.position 
                    + 3 * Vector3.back;
                ceilingTile.ChangeColor(color);
                ceilingTile.transform.parent = parent;
            }
        }

        private void FloodFill(int i, int j, GameObject[,] grid, FloodFillResult result, List<Vector2> visited)
        {
            Debug.Log("Flood fill step (" + i + ", " + j + ")");
            visited.Add(new Vector2(i, j));
            if (IsTileCertainlyOutside(grid, i, j))
            {
                result.IsRoom = false;
            }
            result.Coordinates.Add(new Vector2(i, j));

            Vector2[] neighbors =
            {
                //new Vector2(i - 1, j),
                new Vector2(i + 1, j),
                //new Vector2(i, j - 1),
                new Vector2(i, j + 1)
            };
            foreach (Vector2 neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && CanGetFromTo(grid, i, j, (int) neighbor.x, (int) neighbor.y))
                {
                    FloodFill((int) neighbor.x, (int) neighbor.y, grid, result, visited);
                }
            }
        }

        private bool IsTileCertainlyOutside(GameObject[,] grid, int i, int j)
        {
            string tileName = grid[i, j].name;
            if (i == 0)
            {
                // On the bottom
                if (!tileName.Contains(DOWN) || tileName.Contains(FENCE)) return true;
            }
            else if (i == grid.GetLength(0))
            {
                // On the top
                if (!tileName.Contains(UP) || tileName.Contains(FENCE)) return true;
            }
            else if (j == 0)
            {
                // On the left edge
                if (!tileName.Contains(LEFT) || tileName.Contains(FENCE)) return true;
            }
            else if (j == grid.GetLength(1))
            {
                // On the right edge
                if (!tileName.Contains(RIGHT) || tileName.Contains(FENCE)) return true;
            }
            return false;
        }

        private bool CanGetFromTo(GameObject[,] grid, int fromX, int fromY, int toX, int toY)
        {
            if (toX < 0 || toY < 0 || toX >= grid.GetLength(0) || toY >= grid.GetLength(1))
            {
                return false;
            }
            string fromName = grid[fromX, fromY].name;
            string toName = grid[toX, toY].name;
            if (toX == fromX)
            {
                // UP or DOWN
                if (toY > fromY)
                {
                    // UP
                    return !fromName.Contains(RIGHT) && !toName.Contains(LEFT);
                }
                else
                {
                    // DOWN
                    return !fromName.Contains(LEFT) && !toName.Contains(RIGHT);
                }
            }
            else if (toY == fromY)
            {
                // LEFT or RIGHT
                if (toX > fromX)
                {
                    // RIGHT
                    
                    return !fromName.Contains(UP) && !toName.Contains(DOWN);
                }
                else
                {
                    // LEFT
                    
                    return !fromName.Contains(DOWN) && !toName.Contains(UP);
                }
            }
            //else if (toX > fromX)
            //{
            //    // UP-RIGHT or DOWN-RIGHT
            //    if (toY > fromY)
            //    {
            //        // UP-RIGHT
            //    }
            //    else
            //    {
            //        // DOWN-RIGHT
            //    }
            //}
            //else
            //{
            //    // UP-LEFT or DOWN-LEFT
            //    if (toY > fromY)
            //    {
            //        // UP-LEFT
            //    }
            //    else
            //    {
            //        // DOWN-LEFT
            //    }
            //}
            return false;
        }
    }
}
