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
    }
}
