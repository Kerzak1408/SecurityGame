using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Extensions;
using Assets.Scripts.MapEditor.EditorHandlers;
using Assets.Scripts.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public abstract class GridsBrowserBase<TSelectableHandler> : GridBase where TSelectableHandler : BaseHandler
    {

        public GameObject Grids;
        public GameObject ScrollViewContent;
        public GameObject ScrollView;

        protected Dictionary<Button, Map> MapsDictionary;
        protected Dictionary<int, TSelectableHandler> selectableHandlers;
        protected Button SelectedMapButton;

        public float cameraOriginalSize;

        private Vector3 previousMousePosition;

        public string CurrentMapSaveName
        {
            get { return GetSavedName(SelectedMapButton); }
        }

        protected string GetSavedName(Button button)
        {
            return button.GetComponentInChildren<Text>().text.Replace(' ', '_');
        }

        // Use this for initialization
        protected override void Start ()
        {
            base.Start();
            MapsDictionary = new Dictionary<Button, Map>();
            selectableHandlers = new Dictionary<int, TSelectableHandler>();

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
            if (MapsDictionary.Count > 0)
            {
                SelectMap(MapsDictionary.Keys.First());
            }
            
        }
	
        // Update is called once per frame
        protected virtual void Update ()
        {
            if (Input.GetMouseButtonDown(1))
            {
                previousMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 delta = Input.mousePosition - previousMousePosition;
                previousMousePosition = Input.mousePosition;
                Camera.main.transform.position -= 0.02f * delta;
            }
        }

        protected void SelectMap(Button selectedMapButton)
        {
            if (SelectedMapButton != null)
            {
                SelectedMapButton.GetComponent<Image>().color = Color.white;
                HideCurrentMap();
            }
            SelectedMapButton = selectedMapButton;
            Map map = MapsDictionary[SelectedMapButton];
            map.SetActive(true);
            SelectedMapButton.GetComponent<Image>().color = MyColors.LIGHT_SKY_BLUE;
            Camera.main.orthographicSize = cameraOriginalSize * Mathf.Max(map.Width, map.Height) / 10f;
            Vector3 center = map.CenterWorld;
            Camera.main.transform.position = new Vector3(center.x, center.y, Camera.main.transform.position.z);
        }

        protected virtual void SelectMap()
        {
            Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            SelectMap(clickedButton);
        }

        public void DefaultScrollLogic(float scroll, RaycastHit[] raycastHits)
        {
            if (raycastHits.Length != 0)
            {
                GameObject hitObject = raycastHits[0].transform.gameObject;
                if (!hitObject.IsEqualToDescendantOf(ScrollView))
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
            AdjustScrollContentSize();
            newMap.GetComponent<RectTransform>().localScale = Vector3.one;
            newMap.GetComponent<Image>().color = color;
            newMap.onClick.AddListener(SelectMap);
            newMap.GetComponentInChildren<Text>().text = name;
            ScrollView.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
            return newMap;
        }

        protected void AdjustScrollContentSize()
        {
            var contentRectTransform = ScrollViewContent.GetComponent<RectTransform>();
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x,
                ScrollViewContent.transform.childCount *
                ResourcesHolder.Instance.MapButton.GetComponent<RectTransform>().sizeDelta.y);
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

        protected override Map LoadMap(string mapName, Button correspondingButton, bool mapVisible = false)
        {
            var map = base.LoadMap(mapName, correspondingButton);
            map.EmptyParent.transform.parent = Grids.transform;
            MapsDictionary.Add(correspondingButton, map);
            foreach (var entity in map.Entities)
            {
                entity.DeactivateAllScripts();
                entity.DeactivateAllCamerasAndAudioListeners();
            }
            foreach (var tile in map.Tiles)
            {
                tile.DeactivateAllScripts();
            }
            return map;
        }
    }
}
