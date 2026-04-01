using System.Numerics;
using System.Text;
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

        var builder = new StringBuilder();
        void AddText(Vector4 color, string text)
        {
            builder.AppendLine(text);
            ImGui.TextColored(color, text);
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
            AddText(color, $"{(matches ? "[PASS]" : "[  FAIL  ]")}"); ImGui.SameLine();
            AddText(new Vector4(1, 1, 1, 1), log);
            if (!matches)
            {
                var mismatchIdx = 0;
                while (mismatchIdx < log.Length && mismatchIdx < expected.Length && log[mismatchIdx] == expected[mismatchIdx])
                    mismatchIdx++;
                AddText(red, "[EXPECTED]"); ImGui.SameLine();
                AddText(yellow, expected);
                AddText(red, "[MISMATCH]"); ImGui.SameLine();
                AddText(yellow, "^".PadLeft(mismatchIdx+1));
            }
        }
        
        if (ImGui.Button("Copy Logs"))
            ImGui.SetClipboardText(builder.ToString());
        
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 20)
            ImGui.SetScrollHereY(1.0f);
    }
}