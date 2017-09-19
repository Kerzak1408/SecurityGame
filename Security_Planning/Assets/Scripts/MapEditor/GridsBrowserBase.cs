using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class GridsBrowserBase : MonoBehaviour
{

    public GameObject Grids;
    public GameObject ScrollViewContent;
    public GameObject ScrollView;

    protected Dictionary<Button, GameObject[,]> GridsDictionary;
    protected Dictionary<Button, GameObject> GridsParentsDictionary;
    protected Dictionary<Button, Dictionary<Tuple<int,int>, GameObject>> AdditionalObjects;
    protected Button SelectedMapButton;

    protected string MAPS_PATH;
    protected string ENTITIES_PATH;

    protected float cameraPreviousSize;
    protected float cameraOriginalSize;

    private Vector3 previousMousePosition;

    // Use this for initialization
    protected virtual void Start () {
        GridsDictionary = new Dictionary<Button, GameObject[,]>();
        GridsParentsDictionary = new Dictionary<Button, GameObject>();
        AdditionalObjects = new Dictionary<Button, Dictionary<Tuple<int, int>, GameObject>>();
        MAPS_PATH = Application.persistentDataPath + "/Maps";
        ENTITIES_PATH = Application.persistentDataPath + "/Entities";

        if (!Directory.Exists(MAPS_PATH))
        {
            Directory.CreateDirectory(MAPS_PATH);
        }
        var info = new DirectoryInfo(MAPS_PATH);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            Button addedButton = AddMapButton(file.Name.Replace('_', ' '), Color.white);
            LoadMap(file, addedButton);
        }
        cameraOriginalSize = cameraPreviousSize = Camera.main.orthographicSize;
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        else if (Input.GetMouseButtonUp(1))
        {
            RightButtonUpLogicNormalPhase(ray);
        }
        else
        {
            HoverLogic(ray);
        }
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

    private void ScrollLogicNormalPhase(float scroll, Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            if (!HitObject.IsEqualToDescendantOf(ScrollView))
            {
                float potentiallyNewSize = Camera.main.orthographicSize - scroll;
                if (potentiallyNewSize > 1 && potentiallyNewSize < 20)
                {
                    Camera.main.orthographicSize = potentiallyNewSize;
                }
            }
        }
    }

    protected virtual Button AddMapButton(string name, Color color)
    {
        Button newMap = UnityEngine.UI.Button.Instantiate(ResourcesHolder.Instance.MapButton);
        newMap.transform.parent = ScrollViewContent.transform;
        newMap.GetComponent<Image>().color = color;
        newMap.onClick.AddListener(SelectMap);
        newMap.GetComponentInChildren<Text>().text = name;
        return newMap;
    }

    protected virtual void LoadMap(FileInfo fileInfo, Button correspondingButton)
    {
        var fileStream = new FileStream(fileInfo.FullName, FileMode.Open);
        var namesMatrix = Serializer.Instance.Deserialize<string[,]>(fileStream);
        fileStream.Close();
        fileStream = new FileStream(ENTITIES_PATH + "/" + fileInfo.Name, FileMode.Open);
        var namesDictionary = Serializer.Instance.Deserialize<Dictionary<Tuple<int, int>, string>>(fileStream);
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


        var allEntities = ResourcesHolder.Instance.AllEntities;
        var dictionary = new Dictionary<Tuple<int, int>, GameObject>();
        foreach (KeyValuePair<Tuple<int, int>, string> kvPair in namesDictionary)
        {
            var currentName = kvPair.Value;
            var newEntity = allEntities.FindByName(currentName);
            GameObject newObject = Instantiate(newEntity, transform) as GameObject;
            newObject.transform.position = new Vector3(kvPair.Key.Second - width / 2, kvPair.Key.First - height / 2, -1);
            newObject.transform.parent = emptyParent.transform;
            dictionary.Add(kvPair.Key, newObject);
        }
        AdditionalObjects.Add(correspondingButton, dictionary);
    }

    protected void HideCurrentMap()
    {
        if (SelectedMapButton != null)
        {
            var currentParent = GridsParentsDictionary[SelectedMapButton];
            currentParent.SetActive(false);
        }
    }

    protected abstract void HoverEnded();

    protected abstract void LeftButtonUpLogicNormalPhase(Ray ray);

    protected abstract void RightButtonUpLogicNormalPhase(Ray ray);

    protected abstract void HoverLogic(Ray ray);
}
