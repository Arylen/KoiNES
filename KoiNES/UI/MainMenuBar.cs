using ImGuiNET;
using KoiNES.Data;
using NativeFileDialogSharp;

namespace KoiNES.UI;

public class MainMenuBar : IUiElement
{
    public void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open ROM"))
                {
                    var result = Dialog.FileOpen("nes,nes");
                    if (result.IsOk)
                    {
                        var path = result.Path;
                        RecentRoms.Add(path);
                        LoadRom(path);
                    }
                }

                if (ImGui.BeginMenu("Recent ROMs"))
                {
                    var recent = RecentRoms.GetRecentPaths();
                    if (recent.Count == 0)
                    {
                        ImGui.MenuItem("(none)", false);
                    }
                    else
                    {
                        for (var i = recent.Count - 1; i >= 0; i--)
                        {
                            if (ImGui.MenuItem(Path.GetFileName(recent[i])))
                                LoadRom(recent[i]);
                        }
                    }
                    ImGui.EndMenu();
                }

                ImGui.Separator();
                
                if (ImGui.MenuItem("Exit"))
                    Environment.Exit(0);
                
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }

    private void LoadRom(string path)
    {
        Console.WriteLine("Loading " + path);
    }
}