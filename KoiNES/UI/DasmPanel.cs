using KoiNES.Emulation;
using KoiNES.Emulation.Data;

namespace KoiNES.UI;

public class DasmPanel : IPanel
{
    public void Draw(NesVM vm)
    {
        
    }

    private struct DasmData(ushort address, byte[] bytes, string mnemonic) { }

    private DasmData GetDasmAt(NesVM vm, ushort address)
    {
        var opcode = vm.Bus.Read(address);
        
        var instruction = Cpu.Instructions[opcode];

        var addressPlusOne = address + 1;
        var addressPlusTwo = address + 1;
        
        var paddedData = new byte[]
        {
            opcode,
            (byte)(addressPlusOne < ushort.MaxValue ? vm.Bus.Read((ushort)addressPlusOne) : 0xFF),
            (byte)(addressPlusTwo < ushort.MaxValue ? vm.Bus.Read((ushort)addressPlusTwo) : 0xFF),
        };
        
        var mnemonic = string.Format(instruction.Mnemonic, paddedData[1], paddedData[2]);
        
        return new DasmData(address, paddedData.Take(instruction.Length).ToArray(), mnemonic);
    }
}