using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    
    private GameObject HoveredObject;
    private GameObject ClickedObject;

    private Selectable SelectedInputField;
    

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        
        InputWidth.onValidateInput += NumberValidationFunction;
        InputHeight.onValidateInput += NumberValidationFunction;

        InitializePanelGroup(ResourcesHolder.Instance.AllTiles, PanelStart.transform.position, Tiles);
        InitializePanelGroup(ResourcesHolder.Instance.AllEntities, PanelEntitiesStart.transform.position, Entities);

    }

    private void InitializePanelGroup(UnityEngine.Object[] objects, Vector3 startingPosition, GameObject parent)
    {
        foreach (UnityEngine.Object item in objects)
        {
            GameObject newObject = Instantiate(item) as GameObject;
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
        UnityEngine.Object emptySquare = allTiles.FindByName("Ground0");
        var newGrid = new GameObject[height, width];
        GridsDictionary.Add(button, newGrid);
        AdditionalObjects.Add(button, new Dictionary<Tuple<int, int>, GameObject>());
        var newParent = new GameObject();
        newParent.transform.parent = Grids.transform;
        GridsParentsDictionary.Add(button, newParent);

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
        Debug.Log("Panel New Map Form active: " + PanelNewMapForm.activeInHierarchy);
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
        }
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Panel.activeInHierarchy)
            {
                if (scroll != 0)
                {
                    ScrollLogicReplacePhase(scroll, ray, Tiles);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    LeftButtonUpLogicReplacePhase(ray);
                }
            }
            else if (PanelEntities.activeInHierarchy)
            {
                if (scroll != 0)
                {
                    ScrollLogicReplacePhase(scroll, ray, Entities);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    LeftButtonUpLogicEntityPhase(ray);
                }
            }
            else
            {
                base.Update();
            }
        }
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
                var currentGrid = GridsDictionary[SelectedMapButton];
                Tuple<int, int> coords = currentGrid.GetIndices(ClickedObject);
                newObject.transform.position = ClickedObject.transform.position;
                newObject.transform.parent = ClickedObject.transform.parent;
                currentGrid[coords.First, coords.Second] = newObject;
                Panel.SetActive(false);
                Destroy(ClickedObject);
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
                newObject.name = item.name;
                var currentGrid = GridsDictionary[SelectedMapButton];
                var currentDictionary = AdditionalObjects[SelectedMapButton];
                Tuple<int, int> coords = currentGrid.GetIndices(ClickedObject);
                newObject.transform.position = ClickedObject.transform.position;
                newObject.transform.parent = ClickedObject.transform.parent;
                currentDictionary.Add(coords, newObject);
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
                
            Debug.Log("Mouse button up");
            HitObject.GetComponent<Renderer>().material.color = Color.red;
            ClickedObject = HitObject;
            Canvas.SetActive(false);
            Panel.SetActive(true);
            Panel.transform.localScale *= Camera.main.orthographicSize / cameraPreviousSize;
            cameraPreviousSize = Camera.main.orthographicSize;

        }
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

    }

    protected override void HoverLogic(Ray ray)
    {
        if (Panel.activeInHierarchy)
        {

        }
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject HitObject = hit.transform.gameObject;
                if (HitObject != HoveredObject)
                {
                    HoverEnded();
                    var currentParent = GridsParentsDictionary[SelectedMapButton];
                    if (HitObject.IsEqualToChildOf(currentParent))
                    {
                        Debug.Log("Mouse hovering");
                        HoveredObject = HitObject;
                        HoveredObject.GetComponent<Renderer>().material.color = Color.green;
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
            Debug.Log("Hover ended");
            HoveredObject.GetComponent<Renderer>().material.color = Color.white;
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
        var selectedButtonText =  SelectedMapButton.GetComponentInChildren<Text>().text;
        if (selectedButtonText.Contains("*"))
        {
            SelectedMapButton.GetComponentInChildren<Text>().text = selectedButtonText.Remove(selectedButtonText.Length - 1);
        }
        else
        {
            return;
        }
        var currentGrid = GridsDictionary[SelectedMapButton];
        byte[] serializedMap = Serializer.Instance.SerializeGrid(currentGrid);
        string currentMapName = SelectedMapButton.GetComponentInChildren<Text>().text.Replace(' ','_');
        var binaryWriter = new BinaryWriter(new FileStream(MAPS_PATH + "/" + currentMapName, FileMode.Create));
        binaryWriter.Write(serializedMap);
        binaryWriter.Close();
        if (!Directory.Exists(ENTITIES_PATH))
        {
            Directory.CreateDirectory(ENTITIES_PATH);
        }
        byte[] serializedEntities = Serializer.Instance.SerializeEntities(AdditionalObjects[SelectedMapButton]);
        binaryWriter = new BinaryWriter(new FileStream(ENTITIES_PATH + "/" + currentMapName, FileMode.Create));
        binaryWriter.Write(serializedEntities);
        binaryWriter.Close();

        SelectedMapButton.ChangeColor(MyColors.LIGHT_SKY_BLUE);
    }

    public void DeleteMap()
    {
        var mapPath = MAPS_PATH + "/" + SelectedMapButton.GetComponentInChildren<Text>().text;
        if (File.Exists(mapPath))
        {
            File.Delete(mapPath);
        }
        var parent = GridsParentsDictionary[SelectedMapButton];
        Destroy(parent);
        GridsDictionary.Remove(SelectedMapButton);
        GridsParentsDictionary.Remove(SelectedMapButton);
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
        foreach (var button in GridsDictionary.Keys)
        {
            if (button.GetComponentInChildren<Text>().text == buttonName)
            {
                TextError.text = "Such map already exists. Change at least one of: name, width, height.";
                return;
            }
        }
        if (SelectedMapButton != null)
        {
            SelectedMapButton.GetComponent<Image>().color = Color.white;
        }
        
       
        
        SelectedMapButton = AddMapButton(buttonName, MyColors.LIGHT_SKY_BLUE);

        InitializeGrid(width, height, SelectedMapButton);
        SaveMap();

        // To leave AddButton at the last position.
        ButtonAddMap.transform.parent = null;
        ButtonAddMap.transform.parent = ScrollViewContent.transform;
        PanelNewMapForm.SetActive(false);
        Grids.SetActive(true);
    }

    public void CancelMapCreation()
    {
        PanelNewMapForm.SetActive(false);
    }





}
