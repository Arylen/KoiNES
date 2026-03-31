using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;
using KoiNES.Emulation.Data;

namespace KoiNES.UI;

public class NesTestLogPanel : IPanel
{
    private bool _didInit;
    private List<NesTestLog> _logs = new();
    private List<string> _expectedLines = new();
    
    private void Init(NesVM vm)
    {
        vm.OnNesTestLog += (log) =>
        {
            Console.WriteLine($"[NESTEST] {log.ToString()}");
            _logs.Add(log);
        };
        _didInit = true;
        if (File.Exists("Assets/nestest.log")) 
            _expectedLines = File.ReadAllLines("Assets/nestest.log").ToList();
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
        if (ImGui.Button("Copy Logs"))
            ImGui.SetClipboardText(string.Join('\n', _logs.Select(log => log.ToString())));
        
        ImGui.Separator();
        
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

        for (var i = 0; i < _logs.Count; i++)
        {
            var red = new Vector4(1, 0, 0, 1);
            var green = new Vector4(0, 1, 0, 1);
            var yellow = new Vector4(1, 1, 0, 1);
            var log = _logs[i].ToString();
            var expected = _expectedLines[i];
            var matches = log == expected;
            var color = matches ? green : red;
            ImGui.TextColored(color, $"{(matches ? "[PASS]" : "[  FAIL  ]")}"); ImGui.SameLine();
            ImGui.Text(log);
            if (!matches)
            {
                var mismatchIdx = 0;
                while (mismatchIdx < log.Length && mismatchIdx < expected.Length && log[mismatchIdx] == expected[mismatchIdx])
                    mismatchIdx++;
                ImGui.TextColored(red, "[EXPECTED]"); ImGui.SameLine();
                ImGui.TextColored(yellow, expected);
                ImGui.TextColored(red, "[MISMATCH]"); ImGui.SameLine();
                ImGui.TextColored(yellow, "^".PadLeft(mismatchIdx+1));
            }
        }
    }
}