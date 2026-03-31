using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;
using KoiNES.Emulation.Data;

namespace KoiNES.UI;

public class NesTestLogPanel : IPanel
{
    private bool _didInit;
    private List<NesTestLog> _logs = new();
    
    private void Init(NesVM vm)
    {
        vm.OnNesTestLog += (log) =>
        {
            Console.WriteLine($"[NESTEST] {log.ToString()}");
            _logs.Add(log);
        };
    }
    
    public void Draw(NesVM vm)
    {
        if (!_didInit)
            Init(vm);
        if (ImGui.Begin("NES Test Log"))
        {
            DrawLogs(vm);
        }
        ImGui.End();
    }

    private void DrawLogs(NesVM vm)
    {
        if (!vm.NesTestLogMode)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), "VM.NesTestLogMode is set to False, and will not generate logs!");
            return;
        }

        if (_logs.Count == 0)
        {
            ImGui.TextColored(new Vector4(1, 1, 0, 1), "Waiting on logs.");
            return;
        }

        foreach (var log in _logs)
        {
            ImGui.Text(log.ToString());
        }
    }
}