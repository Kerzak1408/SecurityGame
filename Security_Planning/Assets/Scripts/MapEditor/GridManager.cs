using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{

    public InputField InputWidth;
    public InputField InputHeight;
    public InputField InputName;
    public Text TextError;
    public GameObject Grids;
    public GameObject Panel;
    public GameObject PanelStart;
    public GameObject Tiles;
    public GameObject ScrollView;
    public GameObject ScrollViewContent;
    public GameObject ButtonAddMap;
    public GameObject PanelNewMapForm;
    public GameObject Canvas;

    private Dictionary<Button, GameObject[,]> GridsDictionary;
    private Dictionary<Button, GameObject> GridsParentsDictionary;
    
    private GameObject HoveredObject;
    private GameObject ClickedObject;
    private Button SelectedMapButton;
    private Selectable SelectedInputField;

    private bool replacePhase;
    private Vector3 previousMousePosition;
    private float cameraPreviousSize;
    private float cameraOriginalSize;

    private string MAPS_PATH;

    // Use this for initialization
    private void Start ()
    {
        GridsDictionary = new Dictionary<Button, GameObject[,]>();
        GridsParentsDictionary = new Dictionary<Button, GameObject>();
        MAPS_PATH = Application.persistentDataPath + "/Maps";
        if (!Directory.Exists(MAPS_PATH))
        {
            Directory.CreateDirectory(MAPS_PATH);
        }
        var info = new DirectoryInfo(MAPS_PATH);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            Button addedButton = AddMapButton(file.Name.Replace('_',' '), Color.white);
            LoadMap(file.FullName, addedButton);
        }
        
        cameraOriginalSize = cameraPreviousSize = Camera.main.orthographicSize;
        
        InputWidth.onValidateInput += NumberValidationFunction;
        InputHeight.onValidateInput += NumberValidationFunction;

        Vector3 currentPosition = PanelStart.transform.position;
        foreach (UnityEngine.Object item in ResourcesHolder.Instance.AllTiles)
        {
            GameObject newObject = Instantiate(item) as GameObject;
            newObject.transform.position = currentPosition;
            newObject.transform.parent = Tiles.transform;
            newObject.transform.localScale *= 4;
            newObject.name = item.name;
            currentPosition.x += 8;
        }

    }

    private void InitializeGrid(int width, int height, Button button)
    {
        UnityEngine.Object[] allTiles = ResourcesHolder.Instance.AllTiles;
        UnityEngine.Object emptySquare = allTiles.FindByName("Ground0");
        var newGrid = new GameObject[height, width];
        GridsDictionary.Add(button, newGrid);
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
	private void Update ()
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

            if (replacePhase)
            {
                if (scroll != 0)
                {
                    ScrollLogicReplacePhase(scroll, ray);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    LeftButtonUpLogicReplacePhase(ray);
                }
            }
            else
            {
                if (scroll != 0)
                {
                    ScrollLogicNormalPhase(scroll, ray);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    previousMousePosition = Input.mousePosition;
                    HoverEnded();
                }
                else if (Input.GetMouseButton(1))
                {
                    Vector3 delta = Input.mousePosition - previousMousePosition;
                    previousMousePosition = Input.mousePosition;
                    Camera.main.transform.position -= 0.02f * delta;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    LeftButtonUpLogicNormalPhase(ray);
                }
                else
                {
                    HoverLogic(ray);
                }
            }
        }
    }

    private void LeftButtonUpLogicReplacePhase(Ray ray)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            if (HitsChildOf(Tiles, HitObject))
            {
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
                replacePhase = false;
                Canvas.SetActive(true);
            }
        } else
        {
            Panel.SetActive(false);
            replacePhase = false;
            Canvas.SetActive(true);
        }
            
    }

    private void LeftButtonUpLogicNormalPhase(Ray ray)
    {

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;

            HoverEnded();
                
            Debug.Log("Mouse button up");
            HitObject.GetComponent<Renderer>().material.color = Color.red;
            ClickedObject = HitObject;
            replacePhase = true;
            Canvas.SetActive(false);
            var buttonText = SelectedMapButton.GetComponentInChildren<Text>().text;
            if (buttonText[buttonText.Length-1] != '*')
            {
                SelectedMapButton.GetComponentInChildren<Text>().text += "*";
            }
            

            Panel.SetActive(true);
            Panel.transform.localScale *= Camera.main.orthographicSize / cameraPreviousSize;
            cameraPreviousSize = Camera.main.orthographicSize;

        }
    }

    private void HoverLogic(Ray ray)
    {
        if (replacePhase)
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
                    if (HitsChildOf(currentParent, HitObject))
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

    private void HoverEnded()
    {
        if (HoveredObject != null)
        {
            Debug.Log("Hover ended");
            HoveredObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private bool HitsChildOf(GameObject gameObject, GameObject hitObject)
    {
        foreach (Transform transform in gameObject.transform)
        {
            if (transform.gameObject == hitObject)
            {
                return true;
            }
        }
        return false;
    }

    private bool HitsChildOfRecursively(GameObject gameObject, GameObject hitObject)
    {
        foreach (Transform transform in gameObject.transform)
        {
            if (transform.gameObject == hitObject)
            {
                return true;
            }
            if (HitsChildOfRecursively(transform.gameObject, hitObject))
            {
                return true;
            }
        }
        return false;
    }

    private void ScrollLogicReplacePhase(float scroll, Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Tiles.transform.position += new Vector3(Camera.main.orthographicSize/cameraOriginalSize *  25 * scroll, 0, 0);
        }
    }

    private void ScrollLogicNormalPhase(float scroll, Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            if (!HitsChildOfRecursively(ScrollView, HitObject))
            {
                float potentiallyNewSize = Camera.main.orthographicSize - scroll;
                if (potentiallyNewSize > 1 && potentiallyNewSize < 20)
                {
                    Camera.main.orthographicSize = potentiallyNewSize;
                }
            }
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

    private void HideCurrentMap()
    {
        if (SelectedMapButton != null)
        {
            var currentParent = GridsParentsDictionary[SelectedMapButton];
            currentParent.SetActive(false);
        }
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

    private Button AddMapButton(string name, Color color)
    {
        Button newMap = UnityEngine.UI.Button.Instantiate(ResourcesHolder.Instance.MapButton);
        newMap.transform.parent = ScrollViewContent.transform;
        newMap.GetComponent<Image>().color = color;
        newMap.onClick.AddListener(SelectMap);
        newMap.GetComponentInChildren<Text>().text = name;
        return newMap;
    }

    private void SelectMap()
    {
        if (SelectedMapButton != null)
        {
            SelectedMapButton.GetComponent<Image>().color = Color.white;
            HideCurrentMap();
        }
        SelectedMapButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        SelectedMapButton.GetComponent<Image>().color = MyColors.LIGHT_SKY_BLUE;
        GridsParentsDictionary[SelectedMapButton].SetActive(true);
    }

    private void LoadMap(string fileName, Button correspondingButton)
    {
        var fileStream = new FileStream(fileName, FileMode.Open);
        var namesMatrix = Serializer.Instance.Deserialize<String[,]>(fileStream);
        fileStream.Close();
        var allTiles = ResourcesHolder.Instance.AllTiles;

        int width = namesMatrix.GetLength(1);
        int height = namesMatrix.GetLength(0);
        var loadedGrid = new GameObject[height, width];
        GameObject emptyParent = new GameObject();
        emptyParent.transform.parent = Grids.transform;
        emptyParent.SetActive(false);
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                string currentName = namesMatrix[i, j];
                var newTile = allTiles.FindByName(currentName);
                GameObject newObject = Instantiate(newTile, transform) as GameObject;
                loadedGrid[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.name = newTile.name;
                newObject.transform.parent = emptyParent.transform;
            }
        GridsDictionary.Add(correspondingButton, loadedGrid);
        GridsParentsDictionary.Add(correspondingButton, emptyParent);
    }

}
