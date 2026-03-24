namespace KoiNES.Data;

public static class RecentRoms
{
    private const string FilePath = "RecentRoms.json";
    private static List<string> recentPaths = new ();

    static RecentRoms()
    {
        if (!File.Exists(FilePath))
            File.Create(FilePath).Close();
    }
    
    public static void Add(string path)
    {
        recentPaths.Add(path);
        while  (recentPaths.Count > 5)
            recentPaths.RemoveAt(0);
    }

    public static List<string> GetRecentPaths()
    {
        return recentPaths;
    }
}