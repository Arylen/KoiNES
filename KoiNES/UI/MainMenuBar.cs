using ImGuiNET;

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
                    
                }
                
                ImGui.Separator();
                
                if (ImGui.MenuItem("Exit"))
                    Environment.Exit(0);
                
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }
}