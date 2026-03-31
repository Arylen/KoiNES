using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;

namespace KoiNES.UI;

public class MemoryViewPanel : IPanel
{
    private int _rowOffset = 0;
    
    private const int BytesPerRow = 16;
    private const int NumRows = 17;
    
    public void Draw(NesVM vm)
    {
        if (_rowOffset < 0)
            _rowOffset = 0;
        if (_rowOffset >= 0x10000 - (NumRows * BytesPerRow))
            _rowOffset = 0x10000 - (NumRows * BytesPerRow);
        if (ImGui.Begin("Memory View", ImGuiWindowFlags.AlwaysAutoResize))
        {
            DrawControls(vm);
            DrawMemView(vm);
        }
        ImGui.End();
    }

    private void DrawControls(NesVM vm)
    {
        unsafe
        {
            fixed (int* ptr = &_rowOffset)
            {
                var buttonSize = new Vector2(30, 0);
                var didLeftThreeMult = ImGui.Button("<<<", buttonSize); ImGui.SameLine();
                var didLeftMult = ImGui.Button("<<", buttonSize); ImGui.SameLine();
                var didLeft = ImGui.Button("<", buttonSize); ImGui.SameLine();
                ImGui.SetNextItemWidth(80);
                ImGui.InputScalar("##startAddr", ImGuiDataType.S32, (nint)ptr, nint.Zero, nint.Zero, "%04X", ImGuiInputTextFlags.CharsHexadecimal); ImGui.SameLine();
                var didRight = ImGui.Button(">", buttonSize); ImGui.SameLine();
                var didRightMult = ImGui.Button(">>", buttonSize); ImGui.SameLine();
                var didRightThreeMult = ImGui.Button(">>>", buttonSize); ImGui.SameLine();

                if (ImGui.Button($"PC", buttonSize))
                    _rowOffset = vm.CPU.PC & 0xFFF0;

                ImGui.SameLine();
                if (ImGui.BeginCombo("##quickview", "", ImGuiComboFlags.NoPreview))
                {
                    if (ImGui.Selectable("RAM ($0000)")) _rowOffset = 0x0000;
                    if (ImGui.Selectable("Stack ($0100)")) _rowOffset = 0x0100;
                    if (ImGui.Selectable("PPU Regs ($2000)")) _rowOffset = 0x2000;
                    if (ImGui.Selectable("APU/IO ($4000)")) _rowOffset = 0x4000;
                    if (ImGui.Selectable("PRG-RAM ($6000)")) _rowOffset = 0x6000;
                    if (ImGui.Selectable("PRG-ROM ($8000)")) _rowOffset = 0x8000;
                    if (ImGui.Selectable("Vectors ($FFF0)")) _rowOffset = 0xFFF0;
                    ImGui.Separator();
                    if (ImGui.Selectable($"PC (${vm.CPU.PC:X4})")) _rowOffset = vm.CPU.PC & 0xFFF0;
                    if (ImGui.Selectable($"SP ($01{vm.CPU.SP:X2})")) _rowOffset = 0x0100;
                    ImGui.EndCombo();
                }
                
                if (didLeftThreeMult) ChangeRowOffset(-0x100);
                if (didLeftMult) ChangeRowOffset(-0x10);
                if (didLeft) ChangeRowOffset(-1);
                if (didRight) ChangeRowOffset(1);
                if (didRightMult) ChangeRowOffset(0x10);
                if (didRightThreeMult) ChangeRowOffset(0x100);
            }
        }
    }

    private void DrawMemView(NesVM vm)
    {
        var pcHighlightColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        var dataGreyColor = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
        var dataWhiteColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        void DrawRow(ushort rowAddr)
        {
            var ascii = new char[BytesPerRow];
            ImGui.Text($"{rowAddr:X4}"); ImGui.SameLine();
            for (var i = 0; i < BytesPerRow; i++)
            {
                var address = (ushort)(rowAddr + i);
                var dataAtAddress = vm.Bus.Read(address);
                
                var color = dataWhiteColor;
                if (dataAtAddress == 0x00)
                    color = dataGreyColor;
                if (address == vm.CPU.PC)
                    color = pcHighlightColor;
                
                ImGui.TextColored(color, $"{dataAtAddress:X2}");
                ImGui.SameLine();

                ascii[i] = dataAtAddress is >= (byte)' ' and <= (byte)'~' ? (char)dataAtAddress : '.';
            }

            for (var i = 0; i < ascii.Length; i++)
            {
                var color = ascii[i] == '.' ? dataGreyColor : dataWhiteColor;
                ImGui.TextColored(color, ascii[i].ToString());
                if (i != ascii.Length - 1)
                    ImGui.SameLine(0, 0);
            }
        }
        
        ImGui.BeginChild("memview", new Vector2(0, ImGui.GetTextLineHeightWithSpacing() * (NumRows + 2)), ImGuiChildFlags.Borders | ImGuiChildFlags.AutoResizeX);
        ImGui.Text("    "); ImGui.SameLine();
        for (var i = 0; i < BytesPerRow; i++)
        {
            ImGui.Text($"{i:X2}");
            if (i != BytesPerRow - 1)
                ImGui.SameLine();
        }
        for (var i = 0; i < NumRows; i++)
            DrawRow((ushort)(_rowOffset + (i * BytesPerRow)));
        ImGui.EndChild();
    }
    
    private void ChangeRowOffset(int amount)
    {
        _rowOffset += amount * BytesPerRow;
    }
}