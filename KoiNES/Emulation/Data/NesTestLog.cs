using System.Text;

namespace KoiNES.Emulation.Data;

public class NesTestLog
{
    private readonly string _output;
    
    public NesTestLog(
        Cpu cpu,
        Ppu ppu,
        byte[] bytes,
        string mnemonic
    ) {
        var builder = new StringBuilder();

        builder.Append($"{cpu.PC:X4}  ");
        builder.Append(string.Join(" ", bytes.Select(b => $"{b:X2}")).PadRight(10));
        builder.Append($"{mnemonic,-32}");
        builder.Append($"A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.P:X2} SP:{cpu.SP:X2} ");
        builder.Append($"PPU:{ppu.Line,3},{ppu.Dot,3} ");
        builder.Append($"CYC:{cpu.CycleCount}");
        
        _output = builder.ToString();
    }
    
    public override string ToString() => _output;
}