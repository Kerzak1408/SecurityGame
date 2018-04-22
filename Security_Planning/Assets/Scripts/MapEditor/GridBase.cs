using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.FloodFill;
using Assets.Scripts.Algorithms.FloodFill.Results;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using Assets.Scripts.Helpers;
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
        protected virtual void Start ()
        {
            MAPS_PATH = FileHelper.JoinPath(Application.persistentDataPath, "Maps");
            Physics.gravity = Vector3.forward;
        }

        // Update is called once per frame
        void Update ()
        {
		
        }

        protected virtual Map LoadMap(string mapName, UnityEngine.UI.Button correspondingButton = null, bool mapVisible = false)
        {
            mapName =  mapName.Replace(' ', '_');
            var serializer = Serializer.Instance;
            var namesMatrix = serializer.Deserialize<string[,]>(FileHelper.JoinPath(MAPS_PATH, mapName, TILES));
            var entitiesData = serializer.Deserialize<List<Tuple<string, BaseEntityData>>>(FileHelper.JoinPath(MAPS_PATH, mapName, ENTITIES));
            var passwordDictionary = serializer.Deserialize<Dictionary<Tuple<int, int>, string>>(FileHelper.JoinPath(MAPS_PATH, mapName, PASSWORDS));
            var allTiles = ResourcesHolder.Instance.AllTiles;

            //foreach (KeyValuePair<Tuple<int, int>, string> kvPair in passwordDictionary)
            //{
            //    Debug.Log("KEY=" + kvPair.Key + " VALUE=" + kvPair.Value);
            //}

            int width = namesMatrix.GetLength(1);
            int height = namesMatrix.GetLength(0);
            var loadedGrid = new GameObject[height, width];
            GameObject emptyParent = new GameObject();
            emptyParent.SetActive(mapVisible);
            for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                string currentName = namesMatrix[i, j].Trim();
                //Debug.Log("Loading tile = +" + currentName + "+");
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
            var result = new Map(loadedGrid, entities, emptyParent, passwordDictionary, mapName);
            return result;
        }

        protected void GenerateCeiling(GameObject[,] grid, Transform parent)
        {
            var tuples = new List<IntegerTuple>();
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tuples.Add(new IntegerTuple(i, j));
                }
            }

            List<IntegerTupleCluster> clusters =
                FloodFillAlgorithm.GenerateClusters<IntegerTupleCluster, IntegerTuple>(tuples,
                    tuple => GenerateNeighbors(grid, tuple.First, tuple.Second));

            foreach (IntegerTupleCluster cluster in clusters)
            {
                if (cluster.Members.Any(tuple => IsTileCertainlyOutside(grid, tuple)))
                {
                    AddPavementTexture(grid, cluster.Members, "Pavement");
                }
                else
                {
                    AddCeiling(grid, cluster.Members, parent);
                    AddPavementTexture(grid, cluster.Members, "InnerFloor");
                }
            }
        }

        private void AddPavementTexture(GameObject[,] grid, IEnumerable<IntegerTuple> resultCoordinates, string textureName)
        {
            Sprite texture = Resources.Load<Sprite>("Textures/" + textureName);
            texture = Resources.Load<Sprite>("Textures/" + textureName);
            foreach (IntegerTuple coordinate in resultCoordinates)
            {
                GameObject tile = grid[coordinate.First, coordinate.Second];
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                    spriteRenderer.sprite = texture;
                }

            }
        }

        private void AddCeiling(GameObject[,] grid, IEnumerable<IntegerTuple> coordinates, Transform parent)
        {
            Color color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            foreach (IntegerTuple coordinate in coordinates)
            {
                //Debug.Log("Celeing: (" + coordinate.First + ", " + coordinate.Second + ") + color = " + color);
                GameObject ceilingTile =
                    Instantiate(ResourcesHolder.Instance.AllTiles.FindByName(GridManager.EMPTY_SQUARE)) as GameObject;
                ceilingTile.transform.position = grid[coordinate.First, coordinate.Second].transform.position 
                    + 3 * Vector3.back;
                ceilingTile.ChangeColor(color);
                ceilingTile.transform.parent = parent;
                ceilingTile.GetComponent<SpriteRenderer>().material.shader = Shader.Find("Standard");
            }
        }

        private bool IsTileCertainlyOutside(GameObject[,] grid, IntegerTuple indices)
        {
            int i = indices.First;
            int j = indices.Second;
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

        private IEnumerable<IntegerTuple> GenerateNeighbors(GameObject[,] grid, int i, int j)
        {
            List<IntegerTuple> result = new List<IntegerTuple>();
            IntegerTuple[] neighbors =
            {
                new IntegerTuple(i - 1, j),
                new IntegerTuple(i + 1, j),
                new IntegerTuple(i, j - 1),
                new IntegerTuple(i, j + 1)
            };
            foreach (IntegerTuple neighbor in neighbors)
            {
                if (CanGetFromTo(grid, new IntegerTuple(i, j), neighbor))
                {
                    result.Add(neighbor);
                }
            }

            return result;
        }

        private bool CanGetFromTo(GameObject[,] grid, IntegerTuple from, IntegerTuple to)
        {
            int fromX = from.First;
            int fromY = from.Second;
            int toX = to.First;
            int toY = to.Second;

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
            return false;
        }
    }
}
