namespace KoiNES.Emulation.Data;

public struct Mnemonic(Cpu cpu, ushort pc, byte[] data, string format)
{
    public byte[] Data { get; set; } = data;
    public Cpu Cpu { get; set; } = cpu;
    public string Format { get; set; } = format;
    public ushort PC { get; set; } = pc;

    public override string ToString()
    {
        if (Data.Length != 3)
            Data = Data.Concat(new byte[3 - Data.Length]).ToArray();

        var instructionLength = Cpu.Instructions[Data[0]].Length;

        List<(string token, string value)> replacementMap =
        [
            ("{byte}", $"{Data[1]:X2}"),
            ("{byte2}", $"{Data[2]:X2}"),
            ("{word}", $"{Data[2]:X2}{Data[1]:X2}"),
            ("{sbyte}", $"{(sbyte)Data[1]}"),
            ("{sbyte_rel_pc}", $"{(ushort)(PC + instructionLength + (sbyte)Data[1]):X4}"),
            ("{peek_byte}", $"{Cpu.Bus.Read(Data[1]):X2}"),
            ("{peek_word}", $"{Cpu.Bus.Read((ushort)(Data[2] << 8 | Data[1])):X2}"),
            ("{x}", $"{Cpu.X:X2}"),
            ("{y}", $"{Cpu.Y:X2}"),
            ("{a}", $"{Cpu.A:X2}")
        ];
        
        var mnemonic = Format;
        
        foreach (var replacement in replacementMap)
            mnemonic = mnemonic.Replace(replacement.token, replacement.value);

        return mnemonic;
    }
}