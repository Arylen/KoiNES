using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;

namespace KoiNES.UI;

public class VMControlPanel : IPanel
{
    public void Draw(NesVM vm)
    {
        ImGui.SetNextWindowSizeConstraints(new Vector2(120, 100), new Vector2(float.MaxValue, float.MaxValue));
        if (ImGui.Begin("VM Controls"))
        {
            var buttonSize = new Vector2(Math.Max(100f, ImGui.GetContentRegionAvail().X), 0);
            if (ImGui.Button("Reset", buttonSize))
                vm.Reset();
            if (ImGui.Button("Cycle", buttonSize))
                vm.Cycle();
            if (ImGui.Button("Step", buttonSize))
                vm.Step();
            
            if (ImGui.Button("Run", buttonSize))
            {
                vm.Start();
                vm.IsPaused = false;
            }

            if (ImGui.Button("Pause", buttonSize))
            {
                vm.IsPaused = true;
            }
        }
        ImGui.End();
    }
}