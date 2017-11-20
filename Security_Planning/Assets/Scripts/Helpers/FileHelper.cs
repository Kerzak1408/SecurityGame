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

}
