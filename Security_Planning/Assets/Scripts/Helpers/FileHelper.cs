namespace Assets.Scripts.Helpers
{
    public static class FileHelper
    {
        public static string JoinPath(params string[] pathParts)
        {
            string result = pathParts[0];
            for (int i = 1; i < pathParts.Length; i++)
            {
                result += "/";
                result += pathParts[i];
            }
            return result;
        }

        /// <summary>
        /// Returns the fileName from the path that ends with filename and extension.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileNameOnly(string path)
        {
            string[] array = path.Split('\\');
            string nameWithExtension = array[array.Length - 1];
            array = nameWithExtension.Split('.');
            return array[0];
        }

    }
}
