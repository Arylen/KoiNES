using System.Numerics;
using ImGuiNET;

namespace KoiNES.UI;

public class InputWindow() : IUiElement
{
    public void Draw()
    {
        var smallButtonSize = new Vector2(30, 30);
        var longButtonSize = new Vector2(60, 30);
        
        ImGui.Begin($"Controller");
        
        ImGui.Dummy(smallButtonSize); ImGui.SameLine();
        ImGui.Button("^", smallButtonSize);
        
        ImGui.Button("<", smallButtonSize); ImGui.SameLine();
        ImGui.Dummy(smallButtonSize); ImGui.SameLine();
        ImGui.Button(">", smallButtonSize);
        
        ImGui.Dummy(smallButtonSize); ImGui.SameLine();
        ImGui.Button("v", smallButtonSize); ImGui.SameLine();
        ImGui.Dummy(smallButtonSize); ImGui.SameLine();
        ImGui.Dummy(smallButtonSize); ImGui.SameLine();
        ImGui.Button("Select", longButtonSize); ImGui.SameLine();
        ImGui.Button("Start", longButtonSize); ImGui.SameLine();
        ImGui.Dummy(smallButtonSize); ImGui.SameLine();
        ImGui.Button("B", smallButtonSize); ImGui.SameLine();
        ImGui.Button("A", smallButtonSize); ImGui.SameLine();
        
        ImGui.End();
    }
}