using System.IO;

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
