using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class MapChoiceManager : GridsBrowserBase {

        public void LoadGame()
        {
            if (SelectedMapButton != null)
            {
                Scenes.Load(Scenes.MAIN_SCENE, "map", SelectedMapButton.GetComponentInChildren<Text>().text);
            }
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
