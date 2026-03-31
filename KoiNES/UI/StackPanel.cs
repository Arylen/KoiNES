using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;

namespace KoiNES.UI;

public class StackPanel : IPanel
{
    private const int BytesPerRow = 2;
    
    public void Draw(NesVM vm)
    {
        if (ImGui.Begin("Stack"))
        {
            ImGui.Text($"SP: ${vm.CPU.SP:X2} (${0x0100 + vm.CPU.SP:X4})");
            ImGui.Separator();
            
            var grey = new Vector4(0.4f, 0.4f, 0.4f, 1);
            var white = new Vector4(1, 1, 1, 1);
            var highlight = new Vector4(1, 1, 0, 1);
            
            for (var address = 0x01FF; address > 0x0100; address -= BytesPerRow)
            {
                ImGui.Text($"{address:X4}  "); ImGui.SameLine();
                for (var i = 0; i < BytesPerRow; i++)
                {
                    var offset = (ushort)(address - i);
                    var value = vm.Bus.Read(offset);

                    var color = white;
                    if (offset <= 0x100 + vm.CPU.SP)
                        color = grey;
                    if (offset == 0x100 + vm.CPU.SP)
                        color = highlight;

                    ImGui.TextColored(color, $"{value:X2} ");
                    if (i < BytesPerRow - 1)
                        ImGui.SameLine();
                }
            }
        }

        ImGui.End();
    }
}