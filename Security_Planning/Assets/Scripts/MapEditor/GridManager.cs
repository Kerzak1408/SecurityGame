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
    public GameObject Grid;
    public GameObject Panel;
    public GameObject PanelStart;
    public GameObject Tiles;
    public GameObject ScrollView;
    public GameObject ScrollViewContent;
    public GameObject ButtonAddMap;
    public GameObject PanelNewMapForm;

    private GameObject[,] GridObjects;
    
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
        MAPS_PATH = Application.persistentDataPath + "/Maps";
        if (!Directory.Exists(MAPS_PATH))
        {
            Directory.CreateDirectory(MAPS_PATH);
        }
        var info = new DirectoryInfo(MAPS_PATH);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            AddMapButton(file.Name, Color.white);
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

    private void InitializeGrid(int width, int height)
    {
        UnityEngine.Object[] allTiles = ResourcesHolder.Instance.AllTiles;
        UnityEngine.Object emptySquare = allTiles.FindByName("Ground0");
        GridObjects = new GameObject[height, width];

        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                GameObject newObject = Instantiate(emptySquare, transform) as GameObject;
                GridObjects[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.name = emptySquare.name;
            }
    }
	
	// Update is called once per frame
	private void Update ()
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

        if (PanelNewMapForm.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SelectedInputField = SelectedInputField.FindSelectableOnDown();
                if (SelectedInputField == null)
                {
                    SelectedInputField = InputName;
                }
                SelectedInputField.Select();
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
                Tuple<int, int> coords = GridObjects.GetIndices(ClickedObject);
                newObject.transform.position = ClickedObject.transform.position;
                newObject.transform.parent = ClickedObject.transform.parent;
                GridObjects[coords.First, coords.Second] = newObject;
                Panel.SetActive(false);
                Destroy(ClickedObject);
                replacePhase = false;
            }
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
            SelectedMapButton.GetComponent<Image>().color = Color.yellow;

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
                    if (HitsChildOf(Grid, HitObject))
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

    private void RemoveCurrentMap()
    {
        if (GridObjects != null)
        {
            foreach (GameObject tile in GridObjects)
            {
                Destroy(tile);
            }
        }
    }

    public void SaveMap()
    {
        byte[] serializedMap = Serializer.Instance.SerializeGrid(GridObjects);
        string currentMapName = SelectedMapButton.GetComponentInChildren<Text>().text;
        var binaryWriter = new BinaryWriter(new FileStream(MAPS_PATH + "/" + currentMapName, FileMode.Create));
        binaryWriter.Write(serializedMap);
        binaryWriter.Close();
        SelectedMapButton.ChangeColor(MyColors.LIGHT_SKY_BLUE);
    }

    public void AddMap()
    {
        RemoveCurrentMap();
        PanelNewMapForm.SetActive(true);
        InputName.Select();
        SelectedInputField = InputName;
    }

    public void CreateMap()
    {
        if (InputWidth.text.Length == 0 || InputHeight.text.Length == 0 || InputName.text.Length == 0)
        {
            TextError.text = "All fields are obligatory. ";
            return;
        }
        if (SelectedMapButton != null)
        {
            SelectedMapButton.GetComponent<Image>().color = Color.white;
        }
        
        // To leave AddButton at the last position.
        ButtonAddMap.transform.parent = null;
        ButtonAddMap.transform.parent = ScrollViewContent.transform;
        PanelNewMapForm.SetActive(false);

        int width = int.Parse(InputWidth.text);
        int height = int.Parse(InputHeight.text);
        string buttonName = InputName.text + " (" + width + " x " + height + ")";
        SelectedMapButton = AddMapButton(buttonName, MyColors.LIGHT_SKY_BLUE);

        InitializeGrid(width, height);
        SaveMap();
    }

    private Button AddMapButton(string name, Color color)
    {
        Button newMap = UnityEngine.UI.Button.Instantiate(ResourcesHolder.Instance.MapButton);
        newMap.transform.parent = ScrollViewContent.transform;
        newMap.GetComponent<Image>().color = color;
        newMap.onClick.AddListener(LoadMap);
        newMap.GetComponentInChildren<Text>().text = name;
        return newMap;
    }

    private void LoadMap()
    {
        if (SelectedMapButton != null)
        {
            SelectedMapButton.GetComponent<Image>().color = Color.white;
        }
        SelectedMapButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        SelectedMapButton.GetComponent<Image>().color = MyColors.LIGHT_SKY_BLUE;

        RemoveCurrentMap();
        var fileStream = new FileStream(MAPS_PATH + "/" + SelectedMapButton.GetComponentInChildren<Text>().text, FileMode.Open);
        var namesMatrix = Serializer.Instance.Deserialize<String[,]>(fileStream);
        fileStream.Close();
        var allTiles = ResourcesHolder.Instance.AllTiles;

        int width = namesMatrix.GetLength(1);
        int height = namesMatrix.GetLength(0);
        GridObjects = new GameObject[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                string currentName = namesMatrix[i, j];
                var newTile = allTiles.FindByName(currentName);
                GameObject newObject = Instantiate(newTile, transform) as GameObject;
                GridObjects[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.name = newTile.name;
            }
    }

}
