using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class GridManager : GridsBrowserBase
{
    public InputField InputWidth;
    public InputField InputHeight;
    public InputField InputName;
    public Text TextError;
    
    public GameObject Panel;
    public GameObject PanelStart;
    public GameObject Tiles;

    public GameObject PanelEntities;
    public GameObject PanelEntitiesStart;
    public GameObject Entities;

    public GameObject ButtonAddMap;
    public GameObject PanelNewMapForm;
    public GameObject Canvas;
    public GameObject PanelPassword;
    
    private GameObject HoveredObject;
    private GameObject ClickedObject;

    private Selectable SelectedInputField;

    private Vector3 originalPanelScale;
    private Color hoveredObjectOriginalColor;
    private PasswordGate currentPasswordGate;

    private const string EMPTY_SQUARE = "000_Empty";
    private Tuple<int, int> passwordIndices;
    private bool clickProcessed;
    
    

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        InputWidth.onValidateInput += NumberValidationFunction;
        InputHeight.onValidateInput += NumberValidationFunction;

        InitializePanelGroup(ResourcesHolder.Instance.AllTiles, PanelStart.transform.position, Tiles);
        InitializePanelGroup(ResourcesHolder.Instance.AllEntities, PanelEntitiesStart.transform.position, Entities);

        originalPanelScale = Panel.transform.localScale;
    }

    private void InitializePanelGroup(UnityEngine.Object[] objects, Vector3 startingPosition, GameObject parent)
    {
        objects.OrderBy(x => x.name);
        foreach (UnityEngine.Object item in objects)
        {
            GameObject newObject = Instantiate(item) as GameObject;
            newObject.DeactivateAllScripts();
            newObject.transform.position = startingPosition;
            newObject.transform.parent = parent.transform;
            newObject.transform.localScale *= 4;
            newObject.name = item.name;
            newObject.AddComponent<BoxCollider>();
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
        var map = new Map(newGrid, new Dictionary<Tuple<int, int>, GameObject>(), newParent, new Dictionary<Tuple<int, int>, string>());
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
	
	// Update is called once per frame
	protected override void Update ()
    {
        if (clickProcessed)
        {
            clickProcessed = false;
            return;
        }
        if (PanelPassword.activeInHierarchy)
        {
            return;
        }
        if (PanelNewMapForm.activeInHierarchy)
        {
            Debug.Log("Panel New Map Form active");
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("TAB pressed");
                
                SelectedInputField = SelectedInputField.FindSelectableOnDown();
                if (SelectedInputField == null)
                {
                    SelectedInputField = InputName;
                }
                SelectedInputField.Select();
            }
            return;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Panel.activeInHierarchy)
        {
            Debug.Log("Panel phase");
            if (scroll != 0)
            {
                ScrollLogicReplacePhase(scroll, ray, Tiles);
            }
            if (Input.GetMouseButtonUp(0))
            {
                LeftButtonUpLogicReplacePhase(ray);
            }
            return;
        }
        if (PanelEntities.activeInHierarchy)
        {
            if (scroll != 0)
            {
                ScrollLogicReplacePhase(scroll, ray, Entities);
            }
            if (Input.GetMouseButtonUp(0))
            {
                LeftButtonUpLogicEntityPhase(ray);
            }
            return;
        }
        base.Update();

        
    }

    private void LeftButtonUpLogicReplacePhase(Ray ray)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            if (HitObject.IsEqualToChildOf(Tiles))
            {
                FlagCurrentButton();
                UnityEngine.Object item = ResourcesHolder.Instance.AllTiles.FindByName(HitObject.name);
                GameObject newObject = Instantiate(item) as GameObject;
                newObject.name = item.name;
                var currentGrid = MapsDictionary[SelectedMapButton].Tiles;
                Tuple<int, int> coords = currentGrid.GetIndices(ClickedObject);
                newObject.transform.position = ClickedObject.transform.position;
                newObject.transform.parent = ClickedObject.transform.parent;
                currentGrid[coords.First, coords.Second] = newObject;
                Panel.SetActive(false);
                Destroy(ClickedObject);
                Canvas.SetActive(true);
            }
            else
            {
                Panel.SetActive(false);
                Canvas.SetActive(true);
            }
        }
        else
        {
            Panel.SetActive(false);
            Canvas.SetActive(true);
        }  
    }

    private void LeftButtonUpLogicEntityPhase(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            if (HitObject.IsEqualToChildOf(Entities))
            {
                FlagCurrentButton();
                UnityEngine.Object item = ResourcesHolder.Instance.AllEntities.FindByName(HitObject.name);
                GameObject newObject = Instantiate(item) as GameObject;
                newObject.DeactivateAllScripts();
                newObject.name = item.name;
                var currentMap = MapsDictionary[SelectedMapButton];
                var currentGrid = currentMap.Tiles;
                var currentDictionary = currentMap.Entities;
                Tuple<int, int> coords = currentGrid.GetIndices(ClickedObject);
                newObject.transform.position = ClickedObject.transform.position;
                newObject.transform.parent = ClickedObject.transform.parent;
                currentDictionary.Add(coords, newObject);
                PanelEntities.SetActive(false);
                Canvas.SetActive(true);
            } 
            else
            {
                PanelEntities.SetActive(false);
                Canvas.SetActive(true);
            }
        }
        else
        {
            PanelEntities.SetActive(false);
            Canvas.SetActive(true);
        }
    }

    private void FlagCurrentButton()
    {
        var buttonText = SelectedMapButton.GetComponentInChildren<Text>().text;
        if (buttonText[buttonText.Length - 1] != '*')
        {
            SelectedMapButton.GetComponentInChildren<Text>().text += "*";
        }
    }

    protected override void LeftButtonUpLogicNormalPhase(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            HoverEnded();
            Debug.Log("Mouse button up normal phase");
            GameObject panelToBeShown;
            if (HitObject.transform.parent.gameObject.HasScriptOfType<PasswordGate>())
            {
                currentPasswordGate = HitObject.GetComponentInParent<PasswordGate>();
                panelToBeShown = PanelPassword;
                PanelPassword.GetComponentInChildren<InputField>().text = currentPasswordGate.Password;
                var currentMap = MapsDictionary[SelectedMapButton];
                passwordIndices = currentMap.Tiles.GetIndices(HitObject.transform.parent.gameObject);
                MapsDictionary[SelectedMapButton].PasswordDictionary[passwordIndices] = currentPasswordGate.Password;
                Grids.SetActive(true);
            }
            else
            {
                HitObject.ChangeColor(Color.red);
                ClickedObject = HitObject;
                Canvas.SetActive(false);
                panelToBeShown = Panel;
                AdjustPanelToCamera(panelToBeShown);
            }
            panelToBeShown.SetActive(true);
        }
    }

    private void AdjustPanelToCamera(GameObject panel)
    {
        panel.transform.localScale = (Camera.main.orthographicSize / cameraOriginalSize) * originalPanelScale;
        var cameraPosition = Camera.main.transform.position;
        panel.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0);
    }

    protected override void RightButtonUpLogicNormalPhase(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            ClickedObject = hit.transform.gameObject;
        }
        PanelEntities.SetActive(true);
        Canvas.SetActive(false);
        AdjustPanelToCamera(PanelEntities);
    }

    protected override void HoverLogic(Ray ray)
    {
        if (!Panel.activeInHierarchy)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject HitObject = hit.transform.gameObject;
                if (HitObject != HoveredObject)
                {
                    HoverEnded();
                    var currentParent = MapsDictionary[SelectedMapButton].EmptyParent;
                    if (HitObject.IsEqualToChildOf(currentParent) ||
                        HitObject.transform.parent.gameObject.HasScriptOfType<PasswordGate>())
                    {
                        // TODO: change color depending on whether it is 3d or 2d object
                        HoveredObject = HitObject;
                        hoveredObjectOriginalColor = HitObject.GetComponent<Renderer>().material.color;
                        HitObject.ChangeColor(Color.green);
                    }
                }

            }
            else
            {
                HoverEnded();
                HoveredObject = null;
            }
        }
        
    }

    protected override void HoverEnded()
    {
        if (HoveredObject != null)
        {
            HoveredObject.ChangeColor(hoveredObjectOriginalColor);
        }
    }

    private void ScrollLogicReplacePhase(float scroll, Ray ray, GameObject scrolledObject)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            scrolledObject.transform.position += new Vector3(Camera.main.orthographicSize/cameraOriginalSize *  25 * scroll, 0, 0);
        }
    }



    private char NumberValidationFunction(string text, int charIndex, char addedChar)
    {
        Debug.Log("Validating char: " + addedChar);
        if (addedChar >= '0' && addedChar <= '9')
        {
            return addedChar;
        }
        return '\0';
    }



    public void SaveMap()
    {
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
        var currentMap = MapsDictionary[SelectedMapButton];
        var currentGrid = currentMap.Tiles;
        byte[] serializedMap = Serializer.Instance.SerializeGrid(currentGrid);
        string currentMapName = SelectedMapButton.GetComponentInChildren<Text>().text.Replace(' ','_');

        var mapDirectoryPath = FileHelper.JoinPath(MAPS_PATH, currentMapName);

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
        var mapPath = MAPS_PATH + "/" + SelectedMapButton.GetComponentInChildren<Text>().text;
        if (File.Exists(mapPath))
        {
            File.Delete(mapPath);
        }
        var parent = MapsDictionary[SelectedMapButton].EmptyParent;
        Destroy(parent);
        MapsDictionary.Remove(SelectedMapButton);
        Destroy(SelectedMapButton.gameObject);
        SelectedMapButton = null;
    }

    public void AddMap()
    {
        Grids.SetActive(false);
        HideCurrentMap();
        PanelNewMapForm.SetActive(true);
        InputName.Select();
        SelectedInputField = InputName;
    }

    public void CreateMap()
    {
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
        string buttonName = InputName.text + " (" + width + " x " + height + ")";
        if (buttonName.Contains("_") || buttonName.Contains("*"))
        {
            TextError.text = "Characters '_' and '*' are not allowed in the map name. ";
            return;
        }
        foreach (var button in MapsDictionary.Keys)
        {
            if (button.GetComponentInChildren<Text>().text == buttonName)
            {
                TextError.text = "Such map already exists. Change at least one of: name, width, height.";
                return;
            }
        }
        if (SelectedMapButton != null)
        {
            SelectedMapButton.ChangeColor(Color.white);
        }
        SelectedMapButton = AddMapButton(buttonName, MyColors.LIGHT_SKY_BLUE);

        InitializeGrid(width, height, SelectedMapButton);
        SaveMap();

        // To leave AddButton at the last position.
        ButtonAddMap.transform.parent = null;
        ButtonAddMap.transform.parent = ScrollViewContent.transform;
        PanelNewMapForm.SetActive(false);
        Grids.SetActive(true);
        FlagCurrentButton();
    }

    public void CancelMapCreation()
    {
        clickProcessed = true;
        Debug.Log("Cancel button clicked");
        PanelNewMapForm.SetActive(false);
        Grids.SetActive(true);
        ShowCurrentMap();
    }

    public void ChangePassword()
    {
        clickProcessed = true;
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
}
