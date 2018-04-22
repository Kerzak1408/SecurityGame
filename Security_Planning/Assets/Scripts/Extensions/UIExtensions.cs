using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Extensions
{
    public static class UIExtensions
    {
        public static void ChangeColor(this Button button, Color color)
        {
            button.GetComponent<Image>().color = color;
        }
    }
}
