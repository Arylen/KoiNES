using System.Text;

namespace KoiNES.Emulation.Data;

public readonly struct NesTestLog(
    ushort address,
    byte[] bytes,
    string mnemonic,
    byte regA,
    byte regX,
    byte regY,
    byte regP,
    byte regSP,
    int ppuX,
    int ppuY,
    long cycles
) {
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append($"{address:X4}  ");
        builder.Append(string.Join(" ", bytes.Select(b => $"{b:X2}")).PadRight(9));
        builder.Append($"{mnemonic,-32}");
        builder.Append($"A:{regA:X2} X:{regX:X2} Y:{regY:X2} P:{regP:X2} SP:{regSP:X2} ");
        builder.Append($"PPU:{ppuX,3},{ppuY,3} ");
        builder.Append($"CYC:{cycles}");
        
        return builder.ToString();
    }
}