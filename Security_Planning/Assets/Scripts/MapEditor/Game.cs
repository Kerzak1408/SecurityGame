using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class Game : GridBase
    {
        public Text TextMoney;
        public Button ButtonExit;
        public Image CurrentItemIcon;

        public Map Map { get; private set; }
        public Camera MainCamera;
        public Camera ObserverCamera;
        private Vector3? startPointRight;
        private Vector3 previousMousePosition;

        // Use this for initialization
        protected override void Start ()
        {
            base.Start();
            string mapName = Scenes.GetParam("map");
            Map = LoadMap(mapName, mapVisible:true);
            Map.ExtractAIModel();

            GenerateCeiling(Map.Tiles, Map.EmptyParent.transform);

            foreach (Transform transform in Map.EmptyParent.transform)
            {
                if (transform.name != "CCTV")
                {
                    continue;
                }
                Camera cctv =  transform.gameObject.GetComponentInChildren<Camera>();
                if (cctv != null)
                {
                    RenderTexture newRenderTexture = new RenderTexture(256, 256, 16);
                    newRenderTexture.Create();
                    cctv.targetTexture = newRenderTexture;
                    GameObject.Find("Monitor").GetComponent<Renderer>().material.mainTexture = newRenderTexture;
                }
            }

            foreach (GameObject tile in Map.Tiles)
            {
                if (tile.HasScriptOfType<BaseObject>())
                {
                    tile.GetComponent<BaseObject>().StartGame();
                }
            }

            foreach (GameObject entity in Map.Entities)
            {
                BaseEntity baseEntity = entity.GetComponent<BaseEntity>();
                baseEntity.CurrentGame = this;
                baseEntity.StartGame();
            }

            Map.EmptyParent.transform.Rotate(90, 0, 0);
            Map.ExtractAIModel();
            //Map.EmptyParent.transform.eulerAngles = new Vector3(90, Map.EmptyParent.transform.eulerAngles.y, Map.EmptyParent.transform.eulerAngles.z);
        }

        // Update is called once per frame
        private void Update ()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ButtonExit.gameObject.SetActive(!ButtonExit.IsActive());
            }
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                MainCamera.gameObject.SetActive(!MainCamera.gameObject.activeInHierarchy);
                ObserverCamera.gameObject.SetActive(!ObserverCamera.gameObject.activeInHierarchy);
            }
            if (ObserverCamera != null && ObserverCamera.gameObject.activeInHierarchy)
            {
                // ZOOM
                float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
                if (mouseWheel != 0)
                {
                    float potentialSize = ObserverCamera.orthographicSize - 5 * mouseWheel;
                    if (potentialSize >= 1 && potentialSize <= 20)
                    {
                       ObserverCamera.orthographicSize = potentialSize;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    previousMousePosition = Input.mousePosition;
                }
                else if (Input.GetMouseButton(1))
                {
                    Vector3 delta = Input.mousePosition - previousMousePosition;
                    previousMousePosition = Input.mousePosition;
                    ObserverCamera.transform.position -= 0.02f * new Vector3(delta.x, 0, delta.y);
                }

            }
        }

        private void TranslateBoard(Vector3 delta)
        {
            Vector3 result = Map.EmptyParent.transform.position + delta;
            result.y = 0;
            Map.EmptyParent.transform.position = result;
        }

        public void ExitToMenu()
        {
            Scenes.Load(Scenes.MAIN_MENU);
        }

        private void OnDrawGizmos()
        {

            if (Map == null) return;

            Map.ExtractAIModel();

            foreach (TileNode tileModel in Map.AIModel.Tiles)
            {
                Gizmos.color = tileModel.IsDetectable() ? Color.red : Color.green;
                GameObject mapTile = Map.Tiles.Get(tileModel.Position);
                Gizmos.DrawSphere(mapTile.transform.position, 0.1f);
                foreach (TileEdge edge in tileModel.Edges)
                {
                    GameObject neighborTile = Map.Tiles.Get(edge.Neighbor.Position);
                    switch (edge.Type)
                    {
                        case EdgeType.NORMAL: Gizmos.color = Color.green;
                            break;
                        case EdgeType.DOOR: Gizmos.color = Color.magenta;
                            break;
                        case EdgeType.KEY_DOOR: Gizmos.color = Color.yellow;
                            break;
                        case EdgeType.WINDOW: Gizmos.color = Color.blue;
                            break;
                        case EdgeType.FENCE: Gizmos.color = Color.black;
                            break;
                        case EdgeType.CARD_DOOR: Gizmos.color = Color.red;
                            break;
                        default: Gizmos.color = Color.cyan;
                            break;
                    }
                    
                    Gizmos.DrawLine(mapTile.transform.position, neighborTile.transform.position);
                }
            }

            Gizmos.color = Color.red;
            
            TileNode[,] aiModelTiles = Map.AIModel.Tiles;
            var heuristics = new EuclideanHeuristics<TileNode>(Map.Tiles);
            List<TileEdge> path = AStarAlgorithm.AStar<TileNode, TileEdge>(aiModelTiles[0, 0], aiModelTiles[5, 5], heuristics,
                Debug.Log, node => node.IsDetectable()).Edges;
            if (path != null)
            {
                TileNode previousNode = null;
                foreach (TileEdge edge in path)
                {
                    GameObject start = Map.Tiles.Get(edge.Start.Position);
                    GameObject end = Map.Tiles.Get(edge.Neighbor.Position);
                    Gizmos.DrawLine(start.transform.position, end.transform.position);
                }
            }
        }
    }
}
