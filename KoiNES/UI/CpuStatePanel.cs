using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;

namespace KoiNES.UI;

public class CpuStatePanel : IPanel
{
    public void Draw(NesVM vm)
    {
        if (ImGui.Begin("CPU State", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Text($"PC: ${vm.CPU.PC:X4}");
            ImGui.Text($"SP: ${vm.CPU.SP:X2}");
            ImGui.Separator();
            ImGui.Text($"A: ${vm.CPU.A:X2}");
            ImGui.Text($"X: ${vm.CPU.X:X2}");
            ImGui.Text($"Y: ${vm.CPU.Y:X2}");
            ImGui.Separator();
            DrawReg("N", "Negative", vm.CPU.N); ImGui.SameLine();
            DrawReg("V", "Overflow", vm.CPU.V); ImGui.SameLine();
            ImGui.Text("-"); ImGui.SameLine();
            DrawReg("B", "Break", vm.CPU.B); ImGui.SameLine();
            DrawReg("D", "Decimal", vm.CPU.D); ImGui.SameLine();
            DrawReg("I", "Interrupt Disable", vm.CPU.I); ImGui.SameLine();
            DrawReg("Z", "Zero", vm.CPU.Z); ImGui.SameLine();
            DrawReg("C", "Carry", vm.CPU.C);
        }
        
        ImGui.End();
    }

    private void DrawReg(string label, string tooltip, bool value)
    {
        var color = value ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1);
        ImGui.TextColored(color, label);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(tooltip);
    }
}