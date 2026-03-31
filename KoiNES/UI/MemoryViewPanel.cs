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
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Start Address:"); ImGui.SameLine();
                
                var buttonSize = new Vector2(30, 0);
                var didLeftThreeMult = ImGui.Button("<<<", buttonSize); ImGui.SameLine();
                var didLeftMult = ImGui.Button("<<", buttonSize); ImGui.SameLine();
                var didLeft = ImGui.Button("<", buttonSize); ImGui.SameLine();
                ImGui.SetNextItemWidth(80);
                ImGui.InputScalar("##startAddr", ImGuiDataType.S32, (nint)ptr, nint.Zero, nint.Zero, "%04X", ImGuiInputTextFlags.CharsHexadecimal); ImGui.SameLine();
                var didRight = ImGui.Button(">", buttonSize); ImGui.SameLine();
                var didRightMult = ImGui.Button(">>", buttonSize); ImGui.SameLine();
                var didRightThreeMult = ImGui.Button(">>>", buttonSize);
                
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
        var dataGreyColor = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
        var dataWhiteColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        void DrawRow(ushort rowAddr)
        {
            var ascii = new char[BytesPerRow];
            ImGui.Text($"{rowAddr:X4}"); ImGui.SameLine();
            for (var i = 0; i < BytesPerRow; i++)
            {
                var address = (ushort)(rowAddr + i);
                var dataAtAddress = vm.Bus.Read(address, false);
                
                var color = dataAtAddress == 0 ? dataGreyColor : dataWhiteColor;
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