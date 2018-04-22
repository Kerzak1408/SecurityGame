using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Finds the first item in <paramref name="array"/> with the name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Object FindByName(this Object[] array, string name)
        {
            foreach (Object item in array)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
