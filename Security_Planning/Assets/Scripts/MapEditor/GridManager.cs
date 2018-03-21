using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using Assets.Scripts.MapEditor.EditorHandlers;
using Assets.Scripts.Reflection;
using Assets.Scripts.Serialization;
using CustomSerialization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class GridManager : GridsBrowserBase
    {
        public InputField InputWidth;
        public InputField InputHeight;
        public InputField InputName;
        public Text TextError;
        public GameObject ButtonRemoveEntity;
    
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

        public GameObject ButtonAddMap;

        public GameObject PanelNewMapForm;
        public GameObject Canvas;
        public GameObject PanelPassword;
        public GameObject PanelError;
        public Dropdown DropdownMode;
        
    
        
        public GameObject ClickedTile;

        private Vector3 originalPanelScale;
        
        public PasswordGate currentPasswordGate;

        public const string EMPTY_SQUARE = "000_Empty";
        public Tuple<int, int> passwordIndices;

        private GameObject draggedObject;
        public GameObject toBeRemovedEntity;

        private BaseEditorHandler currentEditorHandler;
        private IEnumerable<BaseEditorHandler> editorHandlers;
        private Dictionary<int, BaseUserSelectableHandler> selectableHandlers;
        private BaseEditorHandler previousHandler;
        private string[] affectedCanvasElements = {"Scroll View", "ButtonMenu", "ButtonSave", "ButtonDelete"};
        internal Vector3 newEntityPosition;

        // Use this for initialization
        protected override void Start ()
        {
            base.Start();
            InputWidth.onValidateInput += NumberValidationFunction;
            InputHeight.onValidateInput += NumberValidationFunction;

            InitializePanelGroup(ResourcesHolder.Instance.AllTiles, PanelStart.transform.position, Tiles);
            InitializePanelGroup(ResourcesHolder.Instance.AllEntitiesIcons, PanelEntitiesStart.transform.position, Entities);

            originalPanelScale = Panel.transform.localScale;
            editorHandlers = ReflectiveEnumerator.GetAllImplementationsOfAbstractClass<BaseEditorHandler>(this);
            currentEditorHandler = editorHandlers.First(x => x is DragHandler);

            IEnumerable<BaseUserSelectableHandler> baseUserSelectableHandlers = editorHandlers.OfType<BaseUserSelectableHandler>();
            selectableHandlers = new Dictionary<int, BaseUserSelectableHandler>();
            var options = new List<Dropdown.OptionData>();
            int indexOfDrag = 0;
            foreach (BaseUserSelectableHandler baseUserSelectableHandler in baseUserSelectableHandlers)
            {
                string className = baseUserSelectableHandler.GetType().Name;
                className = className.Substring(0, className.Length - 7);
                className = Regex.Replace(className, "(\\B[A-Z])", " $1");
                selectableHandlers[options.Count] = baseUserSelectableHandler;
                if (baseUserSelectableHandler is DragHandler)
                {
                    indexOfDrag = options.Count;
                }
                options.Add(new Dropdown.OptionData(className));
            }
            DropdownMode.options = options;
            DropdownMode.value = indexOfDrag;

            if (MapsDictionary.Count > 0)
            {
                DropdownMode.gameObject.SetActive(true);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (eventProcessedByUI)
            {
                eventProcessedByUI = false;
                return;
            }

            base.Update();

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
            BaseUserSelectableHandler handler = selectableHandlers[DropdownMode.value];
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
            var map = new Map(newGrid, new List<GameObject>(), newParent, new Dictionary<Tuple<int, int>, string>());
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
            float cameraRatio = Camera.main.orthographicSize / cameraOriginalSize;
            panel.transform.localScale =  cameraRatio * originalPanelScale;
            yTranslation *= cameraRatio;
            var cameraPosition = Camera.main.transform.position;
            panel.transform.position = new Vector3(cameraPosition.x, cameraPosition.y + yTranslation, 0);
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
                ShowError("Unable to save a map without Guard. ");
                return;
            }
            eventProcessedByUI = true;
            var selectedButtonText =  SelectedMapButton.GetComponentInChildren<Text>().text;
            if (selectedButtonText.Contains("*"))
            {
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
            eventProcessedByUI = true;
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
            ChangeEditorHandler<NewMapHandler>();
            Grids.SetActive(false);
            HideCurrentMap();
            PanelNewMapForm.SetActive(true);
            InputName.Select();
        }

        public void CreateMap()
        {
            eventProcessedByUI = true;
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
        }

        public void CancelMapCreation()
        {
            eventProcessedByUI = true;
            Debug.Log("Cancel button clicked");
            PanelNewMapForm.SetActive(false);
            Grids.SetActive(true);
            ShowCurrentMap();
            ChangeEditorHandler(previousHandler);
        }

        public void ChangePassword()
        {
            eventProcessedByUI = true;
            var dict = MapsDictionary[SelectedMapButton].PasswordDictionary;
            var newPassword = PanelPassword.GetComponentInChildren<InputField>().text;
            var oldPassword = dict[passwordIndices];
            if (newPassword != oldPassword)
            {
                FlagCurrentButton();
                currentPasswordGate.Password = newPassword;
                dict[passwordIndices] = newPassword;
            }
            PanelPassword.SetActive(false);
            Grids.SetActive(true);
        }

        public void ShowError(string message)
        {
            PanelError.GetComponentInChildren<Text>().text = message;
            PanelError.SetActive(true);
        }

        public void HideError()
        {
            eventProcessedByUI = true;
            PanelError.SetActive(false);
        }

        public void RemoveEntity()
        {
            eventProcessedByUI = true;
            FlagCurrentButton();
            ButtonRemoveEntity.SetActive(false);
            MapsDictionary[SelectedMapButton].RemoveEntity(toBeRemovedEntity);
            Destroy(toBeRemovedEntity);
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
        }

        public void SetCanvasActive(bool active)
        {
            foreach (string elementName in affectedCanvasElements)
            {
                Canvas.transform.Find(elementName).gameObject.SetActive(active);
            }
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
    }
}
