using System.Text.Json;

namespace KoiNES.Data;

public static class RecentRoms
{
    private const string FilePath = "RecentRoms.json";
    private static List<string> recentPaths = new ();

    static RecentRoms()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            recentPaths = JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
    }

    public static void Add(string path)
    {
        // Remove before adding so that the new entry's at the top.
        recentPaths.Remove(path);
        recentPaths.Add(path);
        
        while (recentPaths.Count > 5)
            recentPaths.RemoveAt(0);

        File.WriteAllText(FilePath, JsonSerializer.Serialize(recentPaths));
    }

    public static List<string> GetRecentPaths() => recentPaths;
}