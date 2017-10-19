using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class GridsBrowserBase : GridBase
{

    public GameObject Grids;
    public GameObject ScrollViewContent;
    public GameObject ScrollView;

    protected Dictionary<Button, Map> MapsDictionary;

    protected Button SelectedMapButton;
    
    protected float cameraOriginalSize;
    protected bool eventProcessedByUI;

    private Vector3 previousMousePosition;
    

    // Use this for initialization
    protected override void Start () {
        base.Start();
        MapsDictionary = new Dictionary<Button, Map>();

        if (!Directory.Exists(MAPS_PATH))
        {
            Directory.CreateDirectory(MAPS_PATH);
        }
        var info = new DirectoryInfo(MAPS_PATH);
        var directoryInfo = info.GetDirectories();
        foreach (var directory in directoryInfo)
        {
            Button addedButton = AddMapButton(directory.Name.Replace('_', ' '), Color.white);
            LoadMap(directory.Name, addedButton);
        }
        cameraOriginalSize = Camera.main.orthographicSize;
    }
	
	// Update is called once per frame
	protected virtual void Update () {

        if (eventProcessedByUI)
        {
            eventProcessedByUI = false;
            return;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (scroll != 0)
        {

            ScrollLogicNormalPhase(scroll, ray);
        }
        if (Input.GetMouseButtonDown(2))
        {
            previousMousePosition = Input.mousePosition;
            HoverEnded();
        }
        else if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;
            previousMousePosition = Input.mousePosition;
            Camera.main.transform.position -= 0.02f * delta;
        }
        else if (Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1))
        {
            //if (eventProcessedByUI)
            //{
            //    eventProcessedByUI = false;
            //    return;
            //}
            LeftButtonUpLogicNormalPhase(ray);
        }
        else if (Input.GetMouseButtonUp(1) && !Input.GetMouseButton(0))
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
        eventProcessedByUI = true;
        Camera.main.transform.position = Vector3.zero;
        Camera.main.orthographicSize = cameraOriginalSize;
        if (SelectedMapButton != null)
        {
            SelectedMapButton.GetComponent<Image>().color = Color.white;
            HideCurrentMap();
        }
        SelectedMapButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        SelectedMapButton.GetComponent<Image>().color = MyColors.LIGHT_SKY_BLUE;
        MapsDictionary[SelectedMapButton].SetActive(true);
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
        Button newMap = Instantiate(ResourcesHolder.Instance.MapButton);
        newMap.transform.parent = ScrollViewContent.transform;
        newMap.GetComponent<Image>().color = color;
        newMap.onClick.AddListener(SelectMap);
        newMap.GetComponentInChildren<Text>().text = name;
        return newMap;
    }

    

    protected void HideCurrentMap()
    {
        if (SelectedMapButton != null)
        {
            MapsDictionary[SelectedMapButton].SetActive(false);
        }
    }

    protected void ShowCurrentMap()
    {
        if (SelectedMapButton != null)
        {
            MapsDictionary[SelectedMapButton].SetActive(true);
        }
    }

    protected abstract void HoverEnded();

    protected abstract void LeftButtonUpLogicNormalPhase(Ray ray);

    protected abstract void RightButtonUpLogicNormalPhase(Ray ray);

    protected abstract void HoverLogic(Ray ray);

    protected override Map LoadMap(string mapName, Button correspondingButton, bool mapVisible = false)
    {
        var map = base.LoadMap(mapName, correspondingButton);
        map.EmptyParent.transform.parent = Grids.transform;
        MapsDictionary.Add(correspondingButton, map);
        foreach (var entity in map.Entities)
        {
            entity.DeactivateAllScripts();
        }
        foreach (var tile in map.Tiles)
        {
            tile.DeactivateAllScripts();
        }
        return map;
    }

    public void Scrolled()
    {
        eventProcessedByUI = true;
    }
}
