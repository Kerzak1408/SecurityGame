using System;
using System.Collections.Generic;
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
                parameters["gameHandler"] = selectableHandlers[DropdownGameMode.value];
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
                selectableHandlers[options.Count] = handler;
                options.Add(new Dropdown.OptionData(handler.Name));
            }

            DropdownGameMode.options = options;
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
        }
    }
}
