using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Characters.Actions;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using Assets.Scripts.Helpers;
using Assets.Scripts.MapEditor.Handlers.EditorHandlers;
using Assets.Scripts.MapEditor.Handlers.GameModes;
using Assets.Scripts.Model;
using Assets.Scripts.Reflection;
using Assets.Scripts.Serialization;
using CustomSerialization;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class GridManager : GridsBrowserBase<BaseUserSelectableHandler>
    {
        public InputField InputWidth;
        public InputField InputHeight;
        public InputField InputName;
        public InputField InputSensitivity;
        public Text TextError;
        public Text TextMaxVisibilityMeasure;
        public Slider SliderMaxVisibility;
        public Image HandleMaxVisibilitySlider;
        public GameObject ButtonRemoveEntity;
        public Button ExportToPngButton;
    
        public GameObject Panel;
        public GameObject PanelStart;
        public GameObject Tiles;

        public GameObject PanelEntities;
        public GameObject PanelEntitiesStart;
        public GameObject Entities;

        public GameObject PanelAllItems;
        public GameObject PanelAllItemsStart;
        public GameObject AllItems;

        public GameObject PanelItems;
        public GameObject PanelItemsStart;
        public GameObject PanelInfo;
        public GameObject Items;

        public GameObject PanelNewMapForm;
        public GameObject Canvas;
        public GameObject PanelPassword;
        public Dropdown DropdownMode;
        public GameObject PanelEditBehaviour;
        public GameObject PanelLegend;
        public GameObject ClickedTile;

        private Vector3 originalPanelScale;
        private List<GameObject> graphDrawingItems;
        private BaseEditorHandler currentEditorHandler;
        private IEnumerable<BaseEditorHandler> editorHandlers;
        private BaseEditorHandler previousHandler;
        private List<BaseAction>[] drawActions;
        private bool isInitialized;
        private LineRenderer[] lineRenderers;

        private readonly string[] affectedCanvasElements =
        {
            "Scroll View", "ButtonMenu", "ButtonSave", "ButtonDelete", "ButtonImport", "ButtonExport",
            "ButtonExportAll", "ButtonExportPng", "Button_Simulate", "PanelInfo", "ButtonAddMap", "DropdownMode"
        };
        public static readonly Color[] Colors = { Color.green, Color.blue, Color.magenta, Color.yellow, Color.red, Color.black };
        public static readonly string EMPTY_SQUARE = "000_Empty";

        public Vector3 NewEntityPosition { get; set; }
        public GameObject ToBeRemovedEntity { get; set; }
        public Tuple<int, int> PasswordIndices { get; set; }
        public PasswordGate CurrentPasswordGate { get; set; }

        // Use this for initialization
        protected override void Start ()
        {
            base.Start();
            lineRenderers = new LineRenderer[NavigationGoal.PATHS_COUNT];
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                GameObject lineRendererObject = InstantiateGameObject(ResourcesHolder.Instance.LineRenderer);
                lineRendererObject.ChangeMaterialAndColor(Colors[i]);
                lineRenderers[i] = lineRendererObject.GetComponent<LineRenderer>();
            }
            graphDrawingItems = new List<GameObject>();
            drawActions = Scenes.GetObjectParam(Scenes.ACTIONS_TO_DRAW) as List<BaseAction>[];
            
            InputWidth.onValidateInput += NumberValidationFunction;
            InputHeight.onValidateInput += NumberValidationFunction;

            InitializePanelGroup(ResourcesHolder.Instance.AllTiles, PanelStart.transform.position, Tiles);
            InitializePanelGroup(ResourcesHolder.Instance.AllEntitiesIcons, PanelEntitiesStart.transform.position, Entities);

            originalPanelScale = Panel.transform.localScale;
            editorHandlers = ReflectiveEnumerator.GetAllImplementationsOfAbstractClass<BaseEditorHandler>(this);
            currentEditorHandler = editorHandlers.First(x => x is DragHandler);

            IEnumerable<BaseUserSelectableHandler> baseUserSelectableHandlers = editorHandlers.OfType<BaseUserSelectableHandler>();
            var options = new List<Dropdown.OptionData>();
            int indexOfDrag = 0;
            foreach (BaseUserSelectableHandler baseUserSelectableHandler in baseUserSelectableHandlers)
            {

                SelectableHandlers[options.Count] = baseUserSelectableHandler;
                if (baseUserSelectableHandler is DragHandler)
                {
                    indexOfDrag = options.Count;
                }
                options.Add(new Dropdown.OptionData(baseUserSelectableHandler.Name));
            }
            DropdownMode.options = options;
            DropdownMode.value = indexOfDrag;

            if (MapsDictionary.Count > 0)
            {
                DropdownMode.gameObject.SetActive(true);
            }
        }

        private void Initialize()
        {
            DrawActions(drawActions);
        }

        private void DrawActions(List<BaseAction>[] actions)
        {
            if (actions == null)
            {
                return;
            }
            PanelLegend.SetActive(true);

            string mapName = (string)Scenes.GetObjectParam(Scenes.MAP);
            SelectMap(MapsDictionary.First(kvPair => kvPair.Value.Name == mapName).Key);
            for (int i = 0; i < actions.Length; i++)
            {
                List<BaseAction> list = actions[i];
                float spaceSize = 0.05f;
                float translation = i * spaceSize - spaceSize * actions.Length / 2;
                Vector3 offset = new Vector3(2 * translation, translation/2, 0);
                foreach (BaseAction action in list)
                {
                    if (action.GetType() == typeof(NavigationAction))
                    {
                        DrawAction((NavigationAction)action, offset, lineRenderers[i]);
                    }
                    else if (action.GetType() == typeof(InteractAction))
                    {
                        DrawAction((InteractAction)action, offset, Colors[i]);
                    }
                }
            }

        }

        protected override void Update()
        {
            base.Update();

            if (!isInitialized)
            {
                Initialize();
                isInitialized = true;
            }

            var pressedKeys = new List<KeyCode>();
            var upKeys = new List<KeyCode>();
            var downKeys = new List<KeyCode>();
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                SaveMap();
            }
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keyCode))
                {
                    pressedKeys.Add(keyCode);
                }
                if (Input.GetKeyDown(keyCode))
                {
                    downKeys.Add(keyCode);
                }
                if (Input.GetKeyUp(keyCode))
                {
                    upKeys.Add(keyCode);
                }
            }
            if (pressedKeys.Count != 0 || upKeys.Count != 0 || downKeys.Count != 0)
            {
                currentEditorHandler.PressedKeys(upKeys.ToArray(), downKeys.ToArray(), pressedKeys.ToArray());
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray);

            if (Input.GetMouseButtonUp(0))
            {
                currentEditorHandler.LeftButtonUp(raycastHits);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                currentEditorHandler.LeftButtonDown(raycastHits);
            }
            else if (Input.GetMouseButton(0))
            {
                currentEditorHandler.LeftButton(raycastHits);
            }
            else
            {
                currentEditorHandler.HoverLogic(raycastHits);
            }

            if (scroll != 0)
            {
                currentEditorHandler.Scroll(scroll, raycastHits);
            }
        }

        public Map GetCurrentMap()
        {
            return SelectedMapButton == null ? null : MapsDictionary[SelectedMapButton];
        }

        public void ChangeEditorHandler<TEditorHandler>() where TEditorHandler : BaseEditorHandler
        {
            ChangeEditorHandler(editorHandlers.First(x => typeof(TEditorHandler) == x.GetType()));
        }

        public void ChangeEditorHandler(Type handlerType)
        {
            ChangeEditorHandler(editorHandlers.First(x => handlerType == x.GetType()));
        }

        public void ChangeEditorHandler(BaseEditorHandler handler)
        {
            currentEditorHandler.End();
            previousHandler = currentEditorHandler;
            currentEditorHandler = handler;
            currentEditorHandler.Start();
        }

        public void ActivatePreviousHandler()
        {
            ChangeEditorHandler(previousHandler);
        }

        public void FlagCurrentButton()
        {
            var buttonText = SelectedMapButton.GetComponentInChildren<Text>().text;
            if (buttonText[buttonText.Length - 1] != '*')
            {
                SelectedMapButton.GetComponentInChildren<Text>().text += "*";
            }
        }

        public void OnDropdownValueChange()
        {
            BaseUserSelectableHandler handler = SelectableHandlers[DropdownMode.value];
            ChangeEditorHandler(handler.GetType());
            DropdownMode.RefreshShownValue();
        }

        public void InitializePanelGroup(UnityEngine.Object[] objects, Vector3 startingPosition, GameObject parent)
        {
            foreach (Transform transform1 in parent.transform)
            {
                Destroy(transform1.gameObject);
            }
            objects.OrderBy(x => x.name);
            foreach (UnityEngine.Object item in objects)
            {
                GameObject newObject = Instantiate(item) as GameObject;
                newObject.transform.position = startingPosition;
                newObject.transform.parent = parent.transform;
                newObject.transform.localScale *= 4;
                newObject.name = item.name;
                newObject.AddComponent<BoxCollider>();
                newObject.DeactivateAllScripts();
                startingPosition.x += 8;
            }
        } 

        private void InitializeGrid(int width, int height, Button button)
        {
            UnityEngine.Object[] allTiles = ResourcesHolder.Instance.AllTiles;
            UnityEngine.Object emptySquare = allTiles.FindByName(EMPTY_SQUARE);
            var newGrid = new GameObject[height, width];

            var newParent = new GameObject();
            newParent.transform.parent = Grids.transform;
            var map = new Map(newGrid, new List<GameObject>(), newParent, new Dictionary<Tuple<int, int>, string>(),
                button.GetComponentInChildren<Text>().text);
            MapsDictionary.Add(button, map);

            for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                GameObject newObject = Instantiate(emptySquare, transform) as GameObject;
                newGrid[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.name = emptySquare.name;
                newObject.transform.parent = newParent.transform;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="yTranslation"> The y translation of the panel in the default position. </param>
        public void AdjustPanelToCamera(GameObject panel, float yTranslation = 0)
        {
            float cameraRatio = Camera.main.orthographicSize / CameraOriginalSize;
            panel.transform.localScale =  cameraRatio * originalPanelScale;
            yTranslation *= cameraRatio;
            var cameraPosition = Camera.main.transform.position;
            panel.transform.position = new Vector3(cameraPosition.x, cameraPosition.y + yTranslation, panel.transform.position.z);
        }

        private char NumberValidationFunction(string text, int charIndex, char addedChar)
        {
            if (addedChar >= '0' && addedChar <= '9')
            {
                return addedChar;
            }
            return '\0';
        }

        public void SaveMap()
        {
            var currentMap = MapsDictionary[SelectedMapButton];
            bool containsGuard = currentMap.Entities.Any(x => x.HasScriptOfType<Guard>());
            if (!containsGuard)
            {
                UserDialog.Instance.ShowOk("Save failed", "Unable to save a map without Guard.");
                return;
            }
            
            if (IsFlagged(SelectedMapButton))
            {
                var selectedButtonText = SelectedMapButton.GetComponentInChildren<Text>().text;
                SelectedMapButton.GetComponentInChildren<Text>().text = selectedButtonText.Remove(selectedButtonText.Length - 1);
            }
            else
            {
                return;
            }
            var currentGrid = currentMap.Tiles;
            byte[] serializedMap = Serializer.Instance.SerializeGrid(currentGrid);

            var mapDirectoryPath = FileHelper.JoinPath(MAPS_PATH, CurrentMapSaveName);

            DirectoryHelper.CreateDirectoryLazy(mapDirectoryPath);

            var tilesPath = FileHelper.JoinPath(mapDirectoryPath, TILES);
            var binaryWriter = new BinaryWriter(new FileStream(tilesPath, FileMode.Create));
            binaryWriter.Write(serializedMap);
            binaryWriter.Close();

            var entitiesPath = FileHelper.JoinPath(mapDirectoryPath, ENTITIES);
            byte[] serializedEntities = Serializer.Instance.SerializeEntities(currentMap.Entities);
            binaryWriter = new BinaryWriter(new FileStream(entitiesPath, FileMode.Create));
            binaryWriter.Write(serializedEntities);
            binaryWriter.Close();

            var passwordsPath = FileHelper.JoinPath(mapDirectoryPath, PASSWORDS);
            byte[] serializedPasswords = Serializer.Instance.Serialize(currentMap.PasswordDictionary);
            binaryWriter = new BinaryWriter(new FileStream(passwordsPath, FileMode.Create));
            binaryWriter.Write(serializedPasswords);
            binaryWriter.Close();

            SelectedMapButton.ChangeColor(MyColors.LIGHT_SKY_BLUE);
        }

        public void DeleteMap()
        {
            UserDialog.Instance.ShowYesNo(
                "Delete map",
                "Are you sure you want to delete the map \"" +
                SelectedMapButton.GetComponentInChildren<Text>().text +
                "\"? This action cannot be undone.",
                DeleteCurrentMap
                );
        }

        private void DeleteCurrentMap()
        {
            var mapPath = FileHelper.JoinPath(MAPS_PATH, CurrentMapSaveName);
            if (Directory.Exists(mapPath))
            {
                Directory.Delete(mapPath, true);
            }
            var parent = MapsDictionary[SelectedMapButton].EmptyParent;
            Destroy(parent);
            MapsDictionary.Remove(SelectedMapButton);
            SelectedMapButton.transform.parent = null;
            Destroy(SelectedMapButton.gameObject);
            SelectedMapButton = null;
            AdjustScrollContentSize();
        }

        public void AddMap()
        {
            ResetGraphDrawing();
            ChangeEditorHandler<NewMapHandler>();
            Grids.SetActive(false);
            HideCurrentMap();
            PanelNewMapForm.SetActive(true);
            InputName.Select();
        }

        public void CreateMap()
        {
            ChangeEditorHandler(previousHandler);
            if (InputWidth.text.Length == 0 || InputHeight.text.Length == 0 || InputName.text.Length == 0)
            {
                TextError.text = "All fields are obligatory.";
                return;
            }
            int width = int.Parse(InputWidth.text);
            int height = int.Parse(InputHeight.text);
            if (width == 0 || height == 0)
            {
                TextError.text = "Both width and height must be greater than 0.";
                return;
            }
            string buttonName = InputName.text;
            if (buttonName.Contains("_") || buttonName.Contains("*"))
            {
                TextError.text = "Characters '_' and '*' are not allowed in the map name. ";
                return;
            }
            foreach (var button in MapsDictionary.Keys)
            {
                string anotherButtonName = button.GetComponentInChildren<Text>().text;
                if (anotherButtonName == buttonName || anotherButtonName == buttonName + "*")
                {
                    TextError.text = "A map with the given name already exists.";
                    return;
                }
            }

            if (SelectedMapButton != null)
            {
                SelectedMapButton.ChangeColor(Color.white);
            }
            SelectedMapButton = AddMapButton(buttonName, MyColors.LIGHT_SKY_BLUE);

            InitializeGrid(width, height, SelectedMapButton);

            // To leave AddButton at the last position.
            //ButtonAddMap.transform.parent = null;
            //ButtonAddMap.transform.parent = ScrollViewContent.transform;
            PanelNewMapForm.SetActive(false);
            Grids.SetActive(true);
            FlagCurrentButton();
            DropdownMode.transform.gameObject.SetActive(true);
            InitializeBasicEntities();
            PlayerPrefs.SetString(Constants.Constants.PLAYER_PREFS_LAST_MAP, GetCurrentMap().Name);
        }

        private void InitializeBasicEntities()
        {
            IEnumerable<UnityEngine.Object> basicEntities = ResourcesHolder.Instance.BasicEntities;
            foreach (UnityEngine.Object basicEntity in basicEntities)
            {
                GameObject newObject = InstantiateGameObject(basicEntity);
                newObject.DeactivateAllScripts();
                newObject.DeactivateAllCamerasAndAudioListeners();
                Map currentMap = GetCurrentMap();
                newObject.transform.position = new Vector3(0,0,0);
                newObject.transform.parent = currentMap.EmptyParent.transform;
                newObject.transform.name = (basicEntity.name + "_" + currentMap.GetNextEntityId());
                currentMap.Entities.Add(newObject);
                newObject.GetComponent<BaseEntity>().PrefabName = basicEntity.name;
            }
        }

        public void CancelMapCreation()
        {
            Debug.Log("Cancel button clicked");
            PanelNewMapForm.SetActive(false);
            Grids.SetActive(true);
            ShowCurrentMap();
            ChangeEditorHandler(previousHandler);
        }

        public void ChangePassword()
        {
            var dict = MapsDictionary[SelectedMapButton].PasswordDictionary;
            var newPassword = PanelPassword.GetComponentInChildren<InputField>().text;
            var oldPassword = dict[PasswordIndices];
            if (newPassword != oldPassword)
            {
                FlagCurrentButton();
                CurrentPasswordGate.Password = newPassword;
                dict[PasswordIndices] = newPassword;
            }
            PanelPassword.SetActive(false);
            Grids.SetActive(true);
        }

        public void RemoveEntity()
        {
            FlagCurrentButton();
            ButtonRemoveEntity.SetActive(false);
            MapsDictionary[SelectedMapButton].Entities.Remove(ToBeRemovedEntity);
            Destroy(ToBeRemovedEntity);
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }

        public GameObject InstantiateGameObject(UnityEngine.Object item)
        {
            return Instantiate(item) as GameObject;
        }

        protected override void SelectMap()
        {
            base.SelectMap();
            DropdownMode.transform.gameObject.SetActive(true);
            ResetGraphDrawing();
        }

        public void SetCanvasActive(bool active)
        {
            foreach (string elementName in affectedCanvasElements)
            {
                Canvas.transform.Find(elementName).gameObject.SetActive(active);
            }
        }

        public void PreviewPhoto(BaseEventData e)
        {
            SetCanvasActive(false);
            PanelInfo.SetActive(true);
            ExportToPngButton.gameObject.SetActive(true);
            WriteToInfoPanel("Export to PNG");
        }

        public void EndPreview(BaseEventData e)
        {
            SetCanvasActive(true);
            WriteToInfoPanel("");
        }

        public void Export()
        {
            ExportMap.Export(FileHelper.JoinPath(MAPS_PATH, CurrentMapSaveName), CurrentMapSaveName);
        }

        public void ExportAll()
        {
            var mapPaths = new List<string>();
            var mapNames = new List<string>();
            foreach (Button button in MapsDictionary.Keys)
            {
                string name = GetSavedName(button);
                mapPaths.Add(FileHelper.JoinPath(MAPS_PATH, name));
                mapNames.Add(name);
            }
            ExportMap.ExportToFolder(mapPaths.ToArray(), mapNames.ToArray());
        }

        public void ExportPng()
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Export PNG", "", GetCurrentMap().Name, "png");
            if (path.Length != 0)
            {
                StartCoroutine(ExportPngCoroutine(path));
            }
        }

        public void Import()
        {
            string[] importedNames = ExportMap.Import(MAPS_PATH);
            foreach (string name in importedNames)
            {
                Button addedButton = AddMapButton(name.Replace('_', ' '), Color.white);
                LoadMap(name, addedButton);
                SelectMap(addedButton);
            }
        }

        public void ClosePanelEditBehaviour()
        {
            EditBehaviourHandler handler = (EditBehaviourHandler) currentEditorHandler;
            foreach (KeyValuePair<Toggle, bool> keyValuePair in handler.PreviousValues)
            {
                if (keyValuePair.Key.isOn != keyValuePair.Value)
                {
                    FlagCurrentButton();
                    break;
                }
            }

            if (SliderMaxVisibility.value != handler.PreviousSliderValue)
            {
                FlagCurrentButton();
            }
            PanelEditBehaviour.SetActive(false);
        }

        private bool IsFlagged(Button button)
        {
            var buttonText = button.GetComponentInChildren<Text>().text;
            return buttonText.Contains("*");
        }

        public void BackToMenu()
        {
            UnsavedChangesDialog(() => Scenes.Load(Scenes.MAIN_MENU), "quit");
        }

        private void UnsavedChangesDialog(Action action, string additionalText)
        {
            if (MapsDictionary.Keys.Any(IsFlagged))
            {
                int changedmapsCount = MapsDictionary.Keys.Count(IsFlagged);
                bool plural = changedmapsCount > 1;
                string thereIsString = plural ? "There are " : "There is ";
                string mapMapsString = plural ? "maps" : "map";
                UserDialog.Instance.ShowYesNo(
                    "Unsaved changes",
                    thereIsString + changedmapsCount + " " + mapMapsString +
                    " with unsaved changes. Do you want to discard the changes and " + additionalText + "?",
                    action);
            }
            else
            {
                action();
            }
        }

        public void Simulate()
        {
            UnsavedChangesDialog(StartSimulation, "simulate");

        }

        private void StartSimulation()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters[Scenes.GAME_HANDLER] = new SimulationGameHandler();
            parameters[Scenes.MAP] = GetCurrentMap().Name;
            Scenes.Load(Scenes.MAIN_SCENE, parameters);
        }

        public void ResetGraphDrawing()
        {
            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                lineRenderer.positionCount = 0;

            }
            foreach (GameObject gameObject in graphDrawingItems.Copy())
            {
                Destroy(gameObject);
            }
            PanelLegend.SetActive(false);
        }

        private void DrawAction(InteractAction action, Vector3 offset, Color color)
        {
            GameObject vertexObject = InstantiateGameObject(ResourcesHolder.Instance.Vertex);
            vertexObject.ChangeMaterialAndColor(color);
            GameObject interacted = GetCurrentMap().EmptyParent.transform.Find(action.InteractedName).gameObject;
            vertexObject.name = "Vertex_" + interacted.name;
            Vector3 interactedPosition = interacted.transform.position;
            vertexObject.transform.position = new Vector3(interactedPosition.x + offset.x, interactedPosition.y + offset.y, -5);
            graphDrawingItems.Add(vertexObject);
        }

        private void DrawAction(NavigationAction action, Vector3 offset, LineRenderer lineRenderer)
        {
            Queue<TileEdge> pathQueue = action.PathQueue;
            if (pathQueue == null)
            {
                return;
            }
            while (pathQueue.Count > 0)
            {
                TileEdge tileEdge = pathQueue.Dequeue();

                Vector3 startPosition = tileEdge.Start.WorldPosition;
                Vector3 endPosition = tileEdge.Neighbor.WorldPosition;
                if (lineRenderer.positionCount == 0)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(startPosition.x + offset.x, startPosition.z + offset.y, -5));
                }
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount-1, new Vector3(endPosition.x +  offset.x, endPosition.z + offset.y, -5));

            }
        }

        private void OnDrawGizmos()
        {
            Map map = GetCurrentMap();
            if (map == null) return;

            map.EmptyParent.transform.Rotate(90, 0, 0);
            map.ExtractAIModel();
            map.EmptyParent.transform.Rotate(-90, 0, 0);

            foreach (TileNode tileModel in map.AIModel.Tiles)
            {
                Gizmos.color = tileModel.IsDetectable() ? Color.red : Color.green;
                GameObject mapTile = map.Tiles.Get(tileModel.Position);
                Gizmos.DrawSphere(mapTile.transform.position, 0.1f);
                foreach (TileEdge edge in tileModel.Edges)
                {
                    if (edge.IsObstructed(new List<BaseEntity>()))
                    {
                        continue;
                    }
                    GameObject neighborTile = map.Tiles.Get(edge.Neighbor.Position);
                    switch (edge.Type)
                    {
                        case EdgeType.NORMAL:
                            Gizmos.color = Color.green;
                            break;
                        case EdgeType.DOOR:
                            Gizmos.color = Color.magenta;
                            break;
                        case EdgeType.KEY_DOOR:
                            Gizmos.color = Color.yellow;
                            break;
                        case EdgeType.WINDOW:
                            Gizmos.color = Color.blue;
                            break;
                        case EdgeType.FENCE:
                            Gizmos.color = Color.black;
                            break;
                        case EdgeType.CARD_DOOR:
                            Gizmos.color = Color.red;
                            break;
                        default:
                            Gizmos.color = Color.cyan;
                            break;
                    }

                    Gizmos.DrawLine(mapTile.transform.position, neighborTile.transform.position);
                }
            }
        }

        private IEnumerator ExportPngCoroutine(string path)
        {
            SetCanvasActive(false);
            // We should only read the screen buffer after rendering is complete
            yield return new WaitForEndOfFrame();

            // Create a texture the size of the screen, RGB24 format
            Map currentMap = GetCurrentMap();
            GameObject downLeftTile = currentMap.Tiles[0, 0];
            Vector3 downLeftCorner = downLeftTile.transform.position - new Vector3(0.5f, 0.5f);
            GameObject upRightTile = currentMap.Tiles[currentMap.Tiles.GetLength(0) - 1, currentMap.Tiles.GetLength(1) - 1];
            Vector3 upRightCorner = upRightTile.transform.position + new Vector3(0.5f, 0.5f);
            Vector3 screenDownLeft = Camera.main.WorldToScreenPoint(downLeftCorner);
            Vector3 screenUpRight = Camera.main.WorldToScreenPoint(upRightCorner);
            Texture2D tex = new Texture2D((int)(screenUpRight.x - screenDownLeft.x), (int)(screenUpRight.y - screenDownLeft.y), TextureFormat.RGB24, false);

            // Read screen contents into the texture
            tex.ReadPixels(new Rect(screenDownLeft, screenUpRight - screenDownLeft), 0, 0);
            tex.Apply();

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);

            // For testing purposes, also write to a file in the project folder
            File.WriteAllBytes(path, bytes);

            SetCanvasActive(true);
        }

        public void ChangeMaxVisibilityMeasure(float value)
        {
            ((EditBehaviourHandler) currentEditorHandler).ChangeMaxVisibilityMeasure(value);
            FlagCurrentButton();
        }


        public void OnSensitivityEndEdit(string value)
        {
            ((EditBehaviourHandler)currentEditorHandler).ChangeSensitivity(int.Parse(value));
            FlagCurrentButton();
        }

        public void WriteToInfoPanel(string text)
        {
            PanelInfo.GetComponentInChildren<Text>().text = text;
        }
    }
}
