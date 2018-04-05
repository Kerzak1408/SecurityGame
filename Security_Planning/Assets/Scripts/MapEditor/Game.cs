using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class Game : GridBase
    {
        public Text TextMoney;
        public GameObject Menu;
        public Image CurrentItemIcon;
        public GameObject PanelFinishGame;

        public Map Map { get; private set; }
        public bool IsFinished { get; private set; }
        public Camera ObserverCamera;

        public Tuple<Camera, BaseCharacter>[] Cameras;
        private int activeCameraIndex;
        private Vector3? startPointRight;
        private Vector3 previousMousePosition;

        private StreamWriter logFileWriter;
        private BaseGameHandler gameHandler;
        

        // Use this for initialization
        protected override void Start ()
        {
            base.Start();
            var camerasList = new List<Tuple<Camera, BaseCharacter>>();
            camerasList.Add(new Tuple<Camera, BaseCharacter>(ObserverCamera, null));
            string mapName = (string) Scenes.GetObjectParam("map");
            gameHandler = (BaseGameHandler) Scenes.GetObjectParam("gameHandler");
            Map = LoadMap(mapName, mapVisible:true);

            DirectoryHelper.CreateDirectoryLazy(FileHelper.JoinPath(Application.dataPath, "Logs"));
            string logFileName = "Log_" + DateTime.Now.ToString("yyyy-M-d") + "_" + DateTime.Now.ToString("hh-mm-ss") + ".txt";
            logFileWriter = new StreamWriter(FileHelper.JoinPath(Application.dataPath, "Logs", logFileName));
            Log("Simulation of " + Map.Name + " started.");

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
                if (entity.HasScriptOfType<BaseCharacter>())
                {
                    BaseCharacter character = entity.GetComponent<BaseCharacter>();
                    Camera charactersCamera = entity.GetComponentInChildren<Camera>();
                    character.Camera = charactersCamera;
                    camerasList.Add(
                        new Tuple<Camera, BaseCharacter>(charactersCamera, character));
                    if (character is Guard)
                    {
                        activeCameraIndex = camerasList.Count - 1;
                        character.IsActive = true;
                    }
                }
            }

            Cameras = camerasList.ToArray();
            foreach (Tuple<Camera, BaseCharacter> tuple in Cameras)
            {
                tuple.First.gameObject.SetActive(false);
            }
            Cameras[activeCameraIndex].First.gameObject.SetActive(true);

            Map.EmptyParent.transform.Rotate(90, 0, 0);
            Map.ExtractAIModel();
            //Map.EmptyParent.transform.eulerAngles = new Vector3(90, Map.EmptyParent.transform.eulerAngles.y, Map.EmptyParent.transform.eulerAngles.z);

            gameHandler.Start(this);
        }

        private void Update ()
        {
            if (IsFinished)
            {
                return;
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Menu.SetActive(!Menu.activeInHierarchy);
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
            gameHandler.Update();
        }

        public void SwitchCamera()
        {
            SetCameraActive(false);
            activeCameraIndex = (activeCameraIndex + 1) % Cameras.Length;
            SetCameraActive(true);
        }

        private void SetCameraActive(bool active)
        {
            Tuple<Camera, BaseCharacter> tuple = Cameras[activeCameraIndex];
            tuple.First.gameObject.SetActive(active);
            BaseCharacter character = tuple.Second;
            if (character != null)
            {
                character.IsActive = active;
                character.GetActiveItem();
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
            Log("Simulation ended.");
            logFileWriter.Close();
            Scenes.Load(Scenes.MAIN_MENU);
        }

        private void OnDrawGizmos()
        {
            var guardCamera = Cameras.First(kvPair => kvPair.Second != null && kvPair.Second is Guard).First;
            var burglarCollider = Map.Entities.First(entity => entity.HasScriptOfType<Burglar>()).GetComponent<Burglar>().GetComponent<SphereCollider>();
            var planes = GeometryUtility.CalculateFrustumPlanes(guardCamera);
            if (GeometryUtility.TestPlanesAABB(planes, burglarCollider.bounds))
            {
                Vector3 guardCameraPosition = guardCamera.transform.position;

                Ray ray = new Ray(guardCameraPosition, burglarCollider.transform.position + burglarCollider.center - guardCameraPosition);
                Gizmos.DrawRay(ray);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                float minDistance = hits.Min(hit => hit.distance);
                RaycastHit closestHit = hits.First(hit => hit.distance == minDistance);
                Debug.Log("Closest hit: " + closestHit.transform.name);
                if (closestHit.collider == burglarCollider)
                {
                    Debug.Log("Burglar detected.");
                }
            }
            if (Map == null) return;

            Map.ExtractAIModel();

            foreach (TileNode tileModel in Map.AIModel.Tiles)
            {
                Gizmos.color = tileModel.IsDetectable() ? Color.red : Color.green;
                GameObject mapTile = Map.Tiles.Get(tileModel.Position);
                Gizmos.DrawSphere(mapTile.transform.position, 0.1f);
                foreach (TileEdge edge in tileModel.Edges)
                {
                    if (edge.IsObstructed(new List<BaseEntity>()))
                    {
                        continue;
                    }
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
                node => node.IsDetectable()).Edges;
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

        public void Restart()
        {
            Log("Simulation restarted.");
            logFileWriter.Close();
            Scenes.Load(Scenes.MAIN_SCENE, Scenes.Parameters);
        }

        public void Log(string line)
        {
            logFileWriter.WriteLine(line);
            Debug.Log(line);
        }

        public void FinishGame(string message)
        {
            IsFinished = true;
            Log(message);
            PanelFinishGame.SetActive(true);
            PanelFinishGame.GetComponentInChildren<Text>().text = message;
        }

        public void GoalsCompleted(BaseCharacter baseCharacter)
        {
            gameHandler.GoalsCompleted(baseCharacter);
        }
    }
}
