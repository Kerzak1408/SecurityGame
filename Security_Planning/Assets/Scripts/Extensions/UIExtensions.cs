using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions {

    public static void ChangeColor(this Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }
}
