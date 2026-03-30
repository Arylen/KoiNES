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
            ImGui.EndMenu();
        }
    }

    private void OpenRom(NesVM vm)
    {
        var result = Dialog.FileOpen("nes");
        if (!result.IsOk)
            return;
        var data = File.ReadAllBytes(result.Path);
        vm.LoadROM(data);
    }
}