public static class FileHelper {

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

    public static string GetFileNameOnly(string path)
    {
        string[] array = path.Split('\\');
        string nameWithExtension = array[array.Length - 1];
        array = nameWithExtension.Split('.');
        return array[0];
    }

}
