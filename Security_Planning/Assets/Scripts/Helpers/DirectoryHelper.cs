using System.IO;

namespace Assets.Scripts.Helpers
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Creates the directory if it does not already exist.
        /// </summary>
        public static void CreateDirectoryLazy(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
