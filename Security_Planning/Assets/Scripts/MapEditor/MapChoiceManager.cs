using System;
using System.Collections.Generic;
using Assets.Scripts.Extensions;
using Assets.Scripts.MapEditor.Handlers.GameModes;
using Assets.Scripts.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class MapChoiceManager : GridsBrowserBase<BaseGameHandler>
    {
        public Dropdown DropdownGameMode;

        public void LoadGame()
        {
            if (SelectedMapButton != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["gameHandler"] = SelectableHandlers[DropdownGameMode.value];
                parameters["map"] = MapsDictionary[SelectedMapButton].Name;
                Scenes.Load(Scenes.MAIN_SCENE, parameters);
            }
        }

        protected override void Start()
        {
            base.Start();
            IEnumerable<BaseGameHandler> gameHandlers =
                ReflectiveEnumerator.GetAllImplementationsOfAbstractClass<BaseGameHandler>();
            var options = new List<Dropdown.OptionData>();
            foreach (BaseGameHandler handler in gameHandlers)
            {
                SelectableHandlers[options.Count] = handler;
                options.Add(new Dropdown.OptionData(handler.Name));
            }

            DropdownGameMode.options = options;
            DropdownGameMode.value = PlayerPrefs.GetInt(Constants.Constants.PLAYER_PREFS_LAST_GAME_MODE);
        }

        protected override void Update()
        {
            base.Update();
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] raycastHits = Physics.RaycastAll(ray);
                DefaultScrollLogic(scroll, raycastHits);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool down = Input.GetKey(KeyCode.LeftShift);
                DropdownGameMode.ChangeDropdownValue(down);
            }
        }

        public void OnGameModeChange(int value)
        {
            PlayerPrefs.SetInt(Constants.Constants.PLAYER_PREFS_LAST_GAME_MODE, value);
        }
    }
}
