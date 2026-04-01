using System.Numerics;
using ImGuiNET;
using KoiNES.Emulation;
using KoiNES.Emulation.Data;

namespace KoiNES.UI;

public class DasmPanel : IPanel
{
    private const int AmountOfInstructions = 20;
    
    public void Draw(NesVM vm)
    {
        if (ImGui.Begin("Disassembly"))
        {
            var offset = 0;
            for (var i = 0; i < AmountOfInstructions; i++)
            {
                var dasm = GetDasmAt(vm, (ushort)(vm.CPU.PC + offset));
                
                ImGui.Text(i == 0 ? " >" : "  "); ImGui.SameLine();
                ImGui.Text($" {dasm.Address:X4} "); ImGui.SameLine();
                ImGui.Text($" {string.Join(' ', dasm.Bytes.Select(x => $"{x:X2}")),-9} "); ImGui.SameLine();
                
                var mnemonic = dasm.Mnemonic;
                var mnemonicColor = new Vector4(1, 1, 1, 1);
                if (mnemonic == "NOT IMPLEMENTED")
                    mnemonicColor = new Vector4(1, 0.8f, 0, 1);
                ImGui.TextColored(mnemonicColor, mnemonic);
                
                offset += dasm.Bytes.Length;
            }
        }
        ImGui.End();
    }

    private struct DasmData(ushort address, byte[] bytes, string mnemonic)
    {
        public ushort Address { get; set; } = address;
        public byte[] Bytes { get; set; } = bytes;
        public string Mnemonic { get; set; } = mnemonic;
    }

    private DasmData GetDasmAt(NesVM vm, ushort address)
    {
        var opcode = vm.Bus.Read(address);
        
        var instruction = Cpu.Instructions[opcode];

        var addressPlusOne = address + 1;
        var addressPlusTwo = address + 2;
        
        var paddedData = new []
        {
            opcode,
            (byte)(addressPlusOne < ushort.MaxValue ? vm.Bus.Read((ushort)addressPlusOne) : 0xFF),
            (byte)(addressPlusTwo < ushort.MaxValue ? vm.Bus.Read((ushort)addressPlusTwo) : 0xFF),
        };
        
        var mnemonic = new Mnemonic(vm.CPU, address, paddedData, instruction.Mnemonic);
        
        return new DasmData(address, paddedData.Take(instruction.Length).ToArray(), mnemonic.ToString());
    }
}