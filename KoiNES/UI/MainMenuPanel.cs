using ImGuiNET;
using KoiNES.Emulation;
using NativeFileDialogSharp;

namespace KoiNES.UI;

public class MainMenuPanel : IPanel
{
    public void Draw(NesVM vm)
    {
        if (ImGui.BeginMainMenuBar())
        {
            DrawFileMenu(vm);
            DrawMiscMenu(vm);

            ImGui.EndMainMenuBar();
        }
    }

    private void DrawFileMenu(NesVM vm)
    {
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("Open ROM"))
                OpenRom(vm);
            
            if (ImGui.BeginMenu("Recent ROMs"))
            {
                if (RecentRoms.GetRecentPaths().Count == 0)
                    ImGui.MenuItem("No Recent ROMs");
                
                foreach (var path in RecentRoms.GetRecentPaths().ToList())
                {
                    var romName = Path.GetFileNameWithoutExtension(path);
                    if (ImGui.MenuItem(romName))
                        OpenRomByPath(vm, path);
                }
                ImGui.EndMenu();
            }
            
            ImGui.Separator();
            if (ImGui.MenuItem("Exit"))
                Environment.Exit(0);
            
            ImGui.EndMenu();
        }
    }

    private void DrawMiscMenu(NesVM vm)
    {
        if (ImGui.BeginMenu("Misc"))
        {
            if (ImGui.MenuItem("Reset UI Layout"))
            {
                var iniPath = ImGui.GetIO().IniFilename;
                if (File.Exists(iniPath))
                    File.Delete(iniPath);
                ImGui.LoadIniSettingsFromMemory("");
            }
            if (ImGui.MenuItem("Save UI Layout"))
            {
                var iniPath = ImGui.GetIO().IniFilename;
                if (File.Exists(iniPath))
                    File.Delete(iniPath);
                ImGui.SaveIniSettingsToDisk(iniPath);
            }

            var nesTestLogModeVerb = vm.NesTestLogMode ? "Disable" : "Enable";
            if (ImGui.MenuItem($"{nesTestLogModeVerb} NES Test Logs"))
                vm.NesTestLogMode = !vm.NesTestLogMode;
            
            ImGui.EndMenu();
        }
    }

    private void OpenRom(NesVM vm)
    {
        var result = Dialog.FileOpen("nes");
        if (!result.IsOk)
            return;
        OpenRomByPath(vm, result.Path);
    }

    private void OpenRomByPath(NesVM vm, string path)
    {
        if (!File.Exists(path))
            return;
        Console.WriteLine($"Opening ROM at {path}");
        RecentRoms.Add(path);
        var data = File.ReadAllBytes(path);
        vm.LoadROM(data);
    }
}